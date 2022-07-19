using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject player;
    GameManager gameManager;
    GameObject CameraPushBox;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gameManager.player;

        CameraPushBox = Instantiate(Resources.Load("Prefabs/CameraPushBox") as GameObject);
        CameraPushBox.transform.position = player.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newCamPosition = new Vector3(CameraPushBox.transform.position.x, CameraPushBox.transform.position.y + 3, -10);                                 //Moves the camera towards the CameraPushBox
        transform.position = Vector3.MoveTowards(transform.position, newCamPosition, Vector3.Distance(transform.position, newCamPosition) / 5 + 0.3f);         //CameraPushBox is a box of colliders, gets pushed around by the player
    }
}
