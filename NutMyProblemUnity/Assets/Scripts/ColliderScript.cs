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

    private void OnTriggerEnter2D(Collider2D collision)             //checks what which trigger collides with
    {
        switch (parent.tag)
        {
            case "Player":
                switch (tag)
                {
                    case "WeaponTrigger":
                        switch (collision.tag)
                        {
                            case "Enemy":
                                collision.gameObject.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<Weapons>().currentWeapon.iDamage, parent.gameObject);
                                //Debug.Log("enemy hit");
                                break;
                        }
                        break;
                    case "GroundedTrigger":
                        switch (collision.tag)
                        {
                            case "Floor":
                                parent.GetComponent<playerController>().isGrounded = true;
                                break;
                        }
                        break;
                }
                break;


            case "Enemy":
                switch (collision.tag)
                {
                    case "Player":
                        parent.GetComponent<EnemyAttack>().TPlayer.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<EnemyAttack>().iSwordDamage, this.gameObject);
                        break;
                }
                break;


        }

    }
}
