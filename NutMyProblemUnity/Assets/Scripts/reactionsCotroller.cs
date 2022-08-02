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
        if (collision.tag == "WeaponTrigger") Instantiate(Resources.Load<GameObject>("Reactions/reaction"));
        if (collision.tag == "WeaponTrigger") Instantiate(Resources.Load<GameObject>("Reactions/reaction"));
        if (collision.tag == "WeaponTrigger") Instantiate(Resources.Load<GameObject>("Reactions/reaction"));
    }
}
