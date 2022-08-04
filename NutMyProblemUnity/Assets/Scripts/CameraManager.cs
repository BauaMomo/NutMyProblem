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
    [SerializeField] float cameraSmoothing;
    [SerializeField] bool predictingCamera;

    Vector2 oldMoveDir;

    float lastMainXMoveDir;
    float lastXMoveDirChangeTime;
    float lastMainYMoveDir;
    float lastYMoveDirChangeTime;

    float lerpPos;
    bool camMoveStarted = false;
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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (predictingCamera) PredictedMovement();
        else
        {
            Vector3 newCamPosition = new Vector3(CameraPushBox.transform.position.x, CameraPushBox.transform.position.y + cameraYOffset, -10);  //Moves the camera towards the CameraPushBox
            float moveSpeed = Mathf.Pow(Vector3.Distance(transform.position, newCamPosition), 2) / cameraSmoothing;
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
        if(YMoveDir() == 0)
        {
            goal.y = Mathf.Clamp(camLocalPos.y, -3.5f, 3.5f);
            camStartPos.y = camLocalPos.y;
        }

        if (HasMoveDirChanged(mainMoveDir)) StartCamMove();
        if (camMoveStarted) transform.position = MoveCam(goal);
        if (!camMoveStarted) transform.position = goal + player.transform.position;

        /* transform.position = MoveCamToPosition((Vector3)camPosOffset); */
    }

    Vector3 MoveCam(Vector3 _goal)
    {
        if (Vector3.Distance(camLocalPos, _goal) < 0.1f) camMoveStarted = false;

        lerpPos = Mathf.Clamp(lerpPos + 0.05f * cameraSmoothing /* / Vector3.Distance(camStartPos, _goal) */, 0, 1);

        float easedLerpPos = CamSpeed(_goal);

        Vector3 newCamPos = Vector3.Lerp(camStartPos, _goal, easedLerpPos);

        return player.transform.position + newCamPos;
    }

    float CamSpeed(Vector3 _goal)
    {
        float camSpeed = lerpPos * lerpPos * (3 - 2 * lerpPos);

        return camSpeed;
    }

    void StartCamMove()
    {
        camStartPos = camLocalPos;
        camMoveStarted = true;
        lerpPos = 0;
    }

    bool HasMoveDirChanged(Vector2 _newMoveDir)
    {
        bool hasMoveDirChanged = _newMoveDir != oldMoveDir;
        if (hasMoveDirChanged) oldMoveDir = _newMoveDir;
        return hasMoveDirChanged;
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

        if (Time.time > lastXMoveDirChangeTime + 0.2f)
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

        if (Time.time > lastYMoveDirChangeTime + .5f)
        {
            return newMainYMoveDir;
        }
        else return 0;
    }
}
