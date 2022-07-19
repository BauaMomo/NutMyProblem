using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    Transform parent;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = Object.FindObjectOfType<GameManager>();

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
                        if(collision.tag == "CommonKnught" || collision.tag == "Hazardnut") collision.gameObject.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<Weapons>().currentWeapon.iDamage, parent.gameObject);
                        break;
                    case "GroundedTrigger":
                        switch (collision.tag)
                        {
                            case "Floor":
                                parent.GetComponent<playerController>().isGrounded = true;
                                break;
                            case "DeathBarrier": gm.ReloadScene();
                                break;
                        }
                        break;
                }
                break;


            case "CommonKnught":
                switch (collision.tag)
                {
                    case "Player":
                        parent.GetComponent<CommonKnughtController>().TPlayer.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<CommonKnughtController>().iSwordDamage, parent.gameObject);
                        break;
                }
                break;

            case "Hazardnut":
                switch (collision.tag)
                {
                    case "Player":
                        parent.GetComponent<HazardnutController>().TPlayer.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<HazardnutController>().iGlovesDamage, parent.gameObject);
                        break;
                }
                break;


        }

    }
}
