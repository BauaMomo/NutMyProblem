using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject player;
    playerController pCon;
    GameManager gameManager;
    GameObject CameraPushBox;

    [SerializeField] float cameraYOffset;
    [SerializeField] float cameraSpeed;
    [SerializeField] bool predictingCamera;

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

        camLocalPos = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (predictingCamera) PredictedMovement();
        else
        {
            Vector3 newCamPosition = new Vector3(CameraPushBox.transform.position.x, CameraPushBox.transform.position.y + cameraYOffset, -10);  //Moves the camera towards the CameraPushBox
            float moveSpeed = Mathf.Pow(Vector3.Distance(transform.position, newCamPosition), 2) / cameraSpeed;
            transform.position = Vector3.MoveTowards(transform.position, newCamPosition, moveSpeed);         //CameraPushBox is a box of colliders, gets pushed around by the player
            if (Vector2.Distance(player.transform.position, transform.position) > 10f) CameraPushBox.transform.position = player.transform.position;
        }
    }

    void PredictedMovement()
    {
        Vector2 mainMoveDir = new Vector3(XMoveDir(), YMoveDir());
        Vector3 camPosOffset = new Vector3(XMoveDir() * 3, YMoveDir() * 3, -10);
        Vector3 goal = camPosOffset;

        camLocalPos = transform.position - player.transform.position;


        if (XMoveDir() == 0)
        {
            goal.x = Mathf.Clamp(camLocalPos.x, -3, 3);
            camStartPos.x = camLocalPos.x;
        }
        if (YMoveDir() == 0)
        {
            if (pCon.isGrounded)
                goal.y = Mathf.Clamp(camLocalPos.y, 0f, 3f);
            else goal.y = Mathf.Clamp(camLocalPos.y, -3f, 3f);
            camStartPos.y =camLocalPos.y;
        }
        isInFallState = !pCon.isGrounded && pCon.GetComponent<Rigidbody2D>().velocity.y < -2f && Time.time > lastYMoveDirChangeTime + .2f;
        if (isInFallState)
        {
            //Debug.Log("falling");
            goal.y = -2;
            camStartPos.y = camLocalPos.y;
        }
        //if(HasChangedState(ref isInFallState, ref oldIsInFallState)) Debug.Log("isInFallState Changed");
        //if(HasChangedState(ref pCon.isGrounded, ref oldIsGrounded)) Debug.Log("isGroundedState changed");

        if (HasMoveDirXChanged(mainMoveDir)) RestartCamMove(ref camStartPos.x, camLocalPos.x, ref camMoveXStarted, ref lerpPosX);
        if (HasMoveDirYChanged(mainMoveDir)) RestartCamMove(ref camStartPos.y, camLocalPos.y, ref camMoveYStarted, ref lerpPosY);

        Vector3 newLocalCamPos = new Vector3();
        newLocalCamPos.x = MoveCamOnAxis(goal.x, ref camMoveXStarted, camLocalPos.x, camStartPos.x, ref lerpPosX);
        newLocalCamPos.y = MoveCamOnAxis(goal.y, ref camMoveYStarted, camLocalPos.y, camStartPos.y, ref lerpPosY);

        newLocalCamPos = new Vector3(Mathf.Clamp(newLocalCamPos.x, -3, 3), Mathf.Clamp(newLocalCamPos.y, -3f, 3f), -10);

        Vector3 newGlobalCamPos = newLocalCamPos + player.transform.position;
        camLocalPos = newGlobalCamPos - player.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, newGlobalCamPos, Mathf.Pow(Vector3.Distance(transform.position, newGlobalCamPos), 2) / 4);
    }

    float MoveCamOnAxis(float _goal, ref bool _camMoveStarted, float _camLocalPos, float _camStartPos, ref float _lerpPos)
    {
        _lerpPos = Mathf.Clamp(_lerpPos + 0.05f * cameraSpeed /* / Vector3.Distance(camStartPos, _goal) */, 0, 1);
        float easedLerpPos = _lerpPos * _lerpPos * (3 - 2 * _lerpPos);
        float newCamPos = Mathf.Lerp(_camStartPos, _goal, easedLerpPos);

        if (Mathf.Abs(_goal - _camLocalPos) <= 0.1f)
        {
            CancelCamMove(ref _camMoveStarted, ref _lerpPos);
            //Debug.Log("reached goal " + _goal + " whith current pos " + _camLocalPos);
        }
        if (!_camMoveStarted) newCamPos = _goal;

        return newCamPos;
    }

    void RestartCamMove(ref float _camStartPos, float _camLocalPos, ref bool _camMoveStarted, ref float _lerpPos)
    {
        //if(_camMoveStarted) return;

        _camStartPos = _camLocalPos;
        _camMoveStarted = true;
        _lerpPos = 0;
    }

    void CancelCamMove(ref bool _camMoveStarted, ref float _lerpPos)
    {
        _camMoveStarted = false;
        _lerpPos = 0;
    }

    bool HasMoveDirXChanged(Vector2 _newMoveDir)
    {
        bool hasMoveDirChanged = _newMoveDir.x != oldMoveDir.x;
        if (hasMoveDirChanged) oldMoveDir.x = _newMoveDir.x;
        //if (hasMoveDirChanged) Debug.Log("move dir changed");
        return hasMoveDirChanged;
    }

    bool HasMoveDirYChanged(Vector2 _newMoveDir)
    {
        bool hasMoveDirChanged = _newMoveDir.y != oldMoveDir.y || HasChangedState(ref isInFallState, ref oldIsInFallState) || (HasChangedState(ref pCon.isGrounded, ref oldIsGrounded) && pCon.isGrounded);
        if (hasMoveDirChanged) oldMoveDir.y = _newMoveDir.y;
        //if (hasMoveDirChanged) Debug.Log("move dir changed");
        return hasMoveDirChanged;
    }

    bool HasChangedState(ref bool _currentState, ref bool _oldState)
    {
        bool hasChangedState = _currentState != _oldState;
        if (hasChangedState) _oldState = _currentState;
        //if(hasChangedState && isInFallState) Debug.Log("Switched to isInFallState");
        return hasChangedState;
    }

    float XMoveDir()
    {
        float newMainXMoveDir;
        if (Mathf.Abs(pCon.moveVector.x) < 0.2f)
            newMainXMoveDir = 0;
        else newMainXMoveDir = Mathf.Sign(pCon.moveVector.x);

        if (newMainXMoveDir != lastMainXMoveDir)
            lastXMoveDirChangeTime = Time.time;
        lastMainXMoveDir = newMainXMoveDir;

        if (Time.time > lastXMoveDirChangeTime + 0f)
        {
            return newMainXMoveDir;
        }
        else return 0;
    }

    float YMoveDir()
    {
        float newMainYMoveDir;
        if (Mathf.Abs(pCon.moveVector.y) < 0.2f)
            newMainYMoveDir = 0;
        else newMainYMoveDir = Mathf.Sign(pCon.moveVector.y);

        if (newMainYMoveDir != lastMainYMoveDir)
            lastYMoveDirChangeTime = Time.time;
        lastMainYMoveDir = newMainYMoveDir;

        if (Time.time > lastYMoveDirChangeTime + .6f)
        {
            return newMainYMoveDir;
        }
        else return 0;
    }
}
