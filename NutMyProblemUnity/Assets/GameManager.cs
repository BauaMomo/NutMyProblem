using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameObject player;
    GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        player = Instantiate(Resources.Load("prefabs/Player") as GameObject);
        player.transform.position = new Vector2(0,0);

        enemy = Instantiate(Resources.Load("prefabs/commonkught") as GameObject);
        enemy.transform.position = new Vector2(4,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
