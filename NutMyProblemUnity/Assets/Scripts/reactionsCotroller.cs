using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reactionsCotroller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < 3; i++)
        {
            if (collision.tag == "WeaponTrigger")
            {
                GameObject reaction = Instantiate(Resources.Load<GameObject>("prefabs/Reactions/reaction"));
                reaction.transform.position = transform.position;
            }
        }
    }
}
