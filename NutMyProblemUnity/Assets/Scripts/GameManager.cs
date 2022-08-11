using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    GameObject allEnemys;
    List<GameObject> EnemyList = new List<GameObject>();
    public bool changingScene { get; protected set; } = false;




    // Start is called before the first frame update
    void Awake()
    {
        player = transform.Find("Player").gameObject;

        player.transform.parent = null;
        //player.transform.position = new Vector2(0, 0);


    }

    private void Start()
    {
        allEnemys = transform.Find("/Level/Enemies").gameObject;


    }
    void ResetEnemyList()
    {
        EnemyList.Clear();
        for (int i = 0; i < allEnemys.transform.childCount; i++)
        {
            EnemyList.Add(allEnemys.transform.GetChild(i).gameObject);
        }


    }
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Update is called once per frame
    void Update()
    {
        RespawnPlayerOnCheckpoint();
    }

    void RespawnPlayerOnCheckpoint()
    {
        if (player.GetComponent<DamageHandler>().iHealth <= 0)
        {
            ResetEnemyList();

            for (int i = 0; i < EnemyList.Count; i++)
            {
                switch (EnemyList[i].tag)
                {
                    case "Hazardnut":
                        EnemyList[i].GetComponent<HazardnutController>().CheckpointSpawn();
                        break;
                    case "CommonKnught":
                        EnemyList[i].GetComponent<CommonKnughtController>().CheckpointSpawn();
                        break;
                }
            }
            player.GetComponent<DamageHandler>().StartInvincibility(1);
            player.transform.position = playerController.lastCheckpointPosition;
            player.GetComponent<DamageHandler>().ResetHealth();
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
    }
}
