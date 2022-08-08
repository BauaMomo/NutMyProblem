using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reactionPrefab : MonoBehaviour
{
    Sprite Hundred;
    Sprite Flex;
    Sprite Heart;
    Sprite Spark;

    List<Sprite> sprites;

    // Start is called before the first frame update
    void Start()
    {
        Hundred = Resources.Load<Sprite>("prefabs/Reactions/100");
        Flex = Resources.Load<Sprite>("prefabs/Reactions/Flex");
        Heart = Resources.Load<Sprite>("prefabs/Reactions/Heart");
        Spark = Resources.Load<Sprite>("prefabs/Reactions/Spark");

        sprites = new List<Sprite>() { Hundred, Flex, Heart, Spark};

        GetComponent<SpriteRenderer>().sprite = sprites[(int)Random.Range(0,sprites.Count)];
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), 60) * 10);

        Destroy(this.gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
