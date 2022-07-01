using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public bool changingScene { get; protected set; } = false;
    GameObject enemy;
    GameObject enemy2;
    
    
    

    // Start is called before the first frame update
    void Awake()
    {
        player = Instantiate(Resources.Load("prefabs/Player") as GameObject);
        player.transform.position = new Vector2(0, 0);

       enemy = Instantiate(Resources.Load("prefabs/CommonKnught") as GameObject);
        enemy.transform.position = new Vector2(4, 0); 
        
        enemy2 = Instantiate(Resources.Load("prefabs/Hazardnut") as GameObject);
        enemy2.transform.position = new Vector2(12, 0);


    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<DamageHandler>().iHealth <= 0)
        {
            changingScene = true;
            SceneManager.LoadScene("SampleScene");
        }
    }
}
