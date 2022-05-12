using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tag == "WeaponCollider" && collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<Weapons>().Sword.iDamage, parent.gameObject);
            Debug.Log("enemy hit");
        }

        if (tag == "PlayerTrigger" && collision.tag == "Floor") parent.GetComponent<playerController>().isGrounded = true;
        if (tag == "PlayerTrigger" && collision.tag == "Enemy")
        {
            Debug.Log("colloding with enemy");
            Vector2 directionToEnemy = (parent.position - collision.gameObject.transform.position).normalized;
            parent.GetComponent<Rigidbody2D>().AddForce(directionToEnemy * 1500);
        }


    }
}
