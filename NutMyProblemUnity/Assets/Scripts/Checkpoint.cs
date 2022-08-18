using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool CheckpointReached;
    GameManager gm;


    Sprite CheckpointActive;
    Sprite CheckpointInactive;

    private void Start()
    {
        gm = Object.FindObjectOfType<GameManager>();

        CheckpointActive = Resources.Load<Sprite>("Background2/Flag_Checkpoint_Active");
        CheckpointInactive = Resources.Load<Sprite>("Background2/Flag_Checkpoint_Inactive");


    }

    private void Update()
    {
        if (CheckpointReached) GetComponent<SpriteRenderer>().sprite = CheckpointActive;
        else GetComponent<SpriteRenderer>().sprite = CheckpointInactive;
    }

    public void ResetCheckpoint()
    {
        CheckpointReached = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            CheckpointReached = true;
            playerController.lastCheckpointPosition = transform.position;
            gm.ResetCheckpointsExcept(this.gameObject);
        }
    }
}
