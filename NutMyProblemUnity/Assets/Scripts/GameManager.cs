using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public bool changingScene { get; protected set; } = false;
    
    
    

    // Start is called before the first frame update
    void Awake()
    {
        player = Instantiate(Resources.Load("prefabs/Player") as GameObject);
        player.transform.position = new Vector2(0, 0);


    }

    public void ReloadScene()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
