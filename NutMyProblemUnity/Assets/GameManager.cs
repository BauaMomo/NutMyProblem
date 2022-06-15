using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    GameObject enemy;

    // Start is called before the first frame update
    void Awake()
    {
        player = Instantiate(Resources.Load("prefabs/Player") as GameObject);
        player.transform.position = new Vector2(0,0);

        enemy = Instantiate(Resources.Load("prefabs/commonkught") as GameObject);
        enemy.transform.position = new Vector2(4,0);

        GameObject testDrop = Instantiate(Resources.Load("prefabs/WeaponDrop") as GameObject);
        testDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Gloves);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<DamageHandler>().iHealth <= 0)  EditorSceneManager.LoadScene("SampleScene"); 
    }
}
