using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject player;
    playerController pCon;
    GameManager gameManager;
    GameObject CameraPushBox;

    GameObject DebuggingGoal;
    GameObject DebuggingCam;

    [SerializeField] float cameraSpeed;
    [SerializeField] float cameraSmoothing;
    [SerializeField] bool predictingCamera;

    [Header("Debug")]
    [SerializeField] bool camMoveVisualisation;

    Vector2 oldMoveDir;
    bool isInFallState;
    bool oldIsInFallState;
    bool oldIsGrounded;

    float lastMainXMoveDir;
    float lastXMoveDirChangeTime;
    float lastMainYMoveDir;
    float lastYMoveDirChangeTime;

    float lerpPosX;
    float lerpPosY;
    bool camMoveXStarted = false;
    bool camMoveYStarted = false;
    Vector3 camStartPos;
    Vector3 camLocalPos;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gameManager.player;
        pCon = player.GetComponent<playerController>();

        CameraPushBox = Instantiate(Resources.Load("Prefabs/CameraPushBox") as GameObject);
        CameraPushBox.transform.position = player.transform.position;

        DebuggingGoal = transform.Find("DebuggingGoal").gameObject;
        DebuggingCam = transform.Find("DebuggingCam").gameObject;

        DebuggingGoal.transform.parent = null;
        DebuggingCam.transform.parent = null;

        camLocalPos = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (predictingCamera) PredictedMovement();
        else
        {
            Vector3 newCamPosition = new Vector3(CameraPushBox.transform.position.x, CameraPushBox.transform.position.y + 3, -10);  //Moves the camera towards the CameraPushBox
            float moveSpeed = Mathf.Pow(Vector3.Distance(transform.position, newCamPosition), 2) / cameraSpeed;
            transform.position = Vector3.MoveTowards(transform.position, newCamPosition, moveSpeed);         //CameraPushBox is a box of colliders, gets pushed around by the player
            if (Vector2.Distance(player.transform.position, transform.position) > 10f) CameraPushBox.transform.position = player.transform.position;
        }
    }

    void PredictedMovement()
    {
        Vector2 mainMoveDir = new Vector3(XDefaultTaget(), YDefaultTarget());   //Set the direction the player is mainly moving in, depending on how long the player has been moving in the direction
        
        Vector3 localTargetPos = FindTargetPos();   //adjust target pos for different cases

        if (HasMoveDirXChanged(mainMoveDir)) RestartCamMove(ref camStartPos.x, camLocalPos.x, ref camMoveXStarted, ref lerpPosX);  //start moving the camera from the current position if the target changed
        if (HasMoveDirYChanged(mainMoveDir)) RestartCamMove(ref camStartPos.y, camLocalPos.y, ref camMoveYStarted, ref lerpPosY);

        Vector3 newLocalCamPos = new Vector3();
        newLocalCamPos.x = MoveCamOnAxis(localTargetPos.x, ref camMoveXStarted, camLocalPos.x, camStartPos.x, ref lerpPosX);    //move camera to target
        newLocalCamPos.y = MoveCamOnAxis(localTargetPos.y, ref camMoveYStarted, camLocalPos.y, camStartPos.y, ref lerpPosY);

        newLocalCamPos = new Vector3(Mathf.Clamp(newLocalCamPos.x, -3, 3), Mathf.Clamp(newLocalCamPos.y, -3f, 3f), -10);        //limit the camera position to a area around the player

        Vector3 newGlobalCamPos = newLocalCamPos + player.transform.position;   
        camLocalPos = newGlobalCamPos - player.transform.position;

        DebuggingGoal.SetActive(camMoveVisualisation);      //Visualisation for debugging
        DebuggingCam.SetActive(camMoveVisualisation);       //DebuggingCam is the position of the camera before smoothing, DebuggingGoal is the target
        if(camMoveVisualisation)
        {
            DebuggingGoal.transform.position = Vector3.Scale((localTargetPos + player.transform.position), new Vector3(1,1,0));
            DebuggingCam.transform.position = Vector3.Scale(newGlobalCamPos, new Vector3(1,1,0));
        }

        //Smooth out the camera motion. Camera move faster the further it is away from the target
        transform.position = Vector3.MoveTowards(transform.position, newGlobalCamPos, Mathf.Pow(Vector3.Distance(transform.position, newGlobalCamPos), 2) / cameraSmoothing);
    }

    Vector3 FindTargetPos()
    {
        Vector3 targetPos = new Vector3(XDefaultTaget() * 3, YDefaultTarget() * 3, -10);     //the default target position        

        camLocalPos = (transform.position - player.transform.position);     //set the local position of the camera for local movement

        if (XDefaultTaget() == 0)
            targetPos.x = Mathf.Clamp(camLocalPos.x, -3, 3);    //if player isn't moving on x, stop the camera

        if (YDefaultTarget() == 0)      //if player isn't moving on y (yet), clamp the camera around the player
        {
            if (pCon.isGrounded)
                targetPos.y = Mathf.Clamp(camLocalPos.y, 0f, 3f);       //if the player is grounded, move the camera higher
            else targetPos.y = Mathf.Clamp(camLocalPos.y, -3f, 3f);
        }

        isInFallState = !pCon.isGrounded && pCon.GetComponent<Rigidbody2D>().velocity.y < -2f && Time.time > lastYMoveDirChangeTime + .35f;
        if (isInFallState)          //if the player has been falling, lower the camera
            targetPos.y = -2;

        return targetPos;
    }

    float MoveCamOnAxis(float _target, ref bool _camMoveStarted, float _camLocalPos, float _camStartPos, ref float _lerpPos)
    {
        //camera moves on the two axis independantly, otherwise the would be stuttering if the player jumps or otherwise changes the target

        _lerpPos = Mathf.Clamp(_lerpPos + 0.05f * cameraSpeed, 0, 1);
        float easedLerpPos = _lerpPos * _lerpPos * (3 - 2 * _lerpPos);      //eased lerp position for smooth acceleration
        float newCamPos = Mathf.Lerp(_camStartPos, _target, easedLerpPos);

        if (Mathf.Abs(_target - _camLocalPos) <= 0.1f)
            CancelCamMove(ref _camMoveStarted, ref _lerpPos);       //if the camera is close to the target, cancel the move
        
        if (!_camMoveStarted) newCamPos = _target;      //if the camera isn't moving, snap it to the target
        return newCamPos;
    }

    void RestartCamMove(ref float _camStartPos, float _camLocalPos, ref bool _camMoveStarted, ref float _lerpPos)
    {
        _camStartPos = _camLocalPos;
        _camMoveStarted = true;
        _lerpPos = 0;
    }

    void CancelCamMove(ref bool _camMoveStarted, ref float _lerpPos)
    {
        if(!_camMoveStarted) return;

        _camMoveStarted = false;
        _lerpPos = 0;
    }

    bool HasMoveDirXChanged(Vector2 _newMoveDir)
    {
        bool hasMoveDirChanged = _newMoveDir.x != oldMoveDir.x;
        if (hasMoveDirChanged) oldMoveDir.x = _newMoveDir.x;
        return hasMoveDirChanged;
    }

    bool HasMoveDirYChanged(Vector2 _newMoveDir)
    {
        bool hasMoveDirChanged = _newMoveDir.y != oldMoveDir.y || HasChangedState(ref isInFallState, ref oldIsInFallState) || (HasChangedState(ref pCon.isGrounded, ref oldIsGrounded) && pCon.isGrounded);
        if (hasMoveDirChanged) oldMoveDir.y = _newMoveDir.y;
        return hasMoveDirChanged;
    }

    bool HasChangedState(ref bool _currentState, ref bool _oldState)
    {
        bool hasChangedState = _currentState != _oldState;
        if (hasChangedState) _oldState = _currentState;
        return hasChangedState;
    }

    float XDefaultTaget()
    {
        //returns 1, 0 or -1 depending on how long and in which direction the player has been moving in

        float newMainXMoveDir;
        if (Mathf.Abs(pCon.moveVector.x) <= 0f)
            newMainXMoveDir = 0;
        else newMainXMoveDir = Mathf.Sign(pCon.moveVector.x);

        if (newMainXMoveDir != lastMainXMoveDir)
            lastXMoveDirChangeTime = Time.time;
        lastMainXMoveDir = newMainXMoveDir;

        if (Time.time > lastXMoveDirChangeTime + 0f)
            return newMainXMoveDir;            
        else return 0;
    }

    float YDefaultTarget()
    {
        float newMainYMoveDir;
        if (Mathf.Abs(pCon.moveVector.y) <= 0.1f)
            newMainYMoveDir = 0;
        else newMainYMoveDir = Mathf.Sign(pCon.moveVector.y);

        if (newMainYMoveDir != lastMainYMoveDir)
            lastYMoveDirChangeTime = Time.time;
        lastMainYMoveDir = newMainYMoveDir;

        if (Time.time > lastYMoveDirChangeTime + .6f)
            return newMainYMoveDir;
        else return 0;
    }
}
