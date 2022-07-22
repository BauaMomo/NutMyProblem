using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColliderScript : MonoBehaviour
{
    Transform parent;
    GameManager gm;
    BoxCollider2D GroundedTrigger;

    // Start is called before the first frame update
    void Start()
    {
        gm = Object.FindObjectOfType<GameManager>();

        parent = this.transform.parent;
        if(tag == "GroundedTrigger") GroundedTrigger = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        switch (parent.tag)
        {
            case "Player":
                switch (tag)
                {
                    case "GroundedTrigger":
                        if (!GroundedTrigger.IsTouchingLayers(1 << LayerMask.NameToLayer("Floor"))) Invoke("UnGround", 0.1f);
                        break;
                }
                break;
        }
    }

    void UnGround()
    {
        if (parent.GetComponent<playerController>().isGrounded)
        {
            if (!GroundedTrigger.IsTouchingLayers(1 << LayerMask.NameToLayer("Floor")))
            {
                //Debug.Log("Ungrounding");
                parent.GetComponent<playerController>().isGrounded = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)             //checks what which trigger collides with
    {
        switch (parent.tag)
        {
            case "Player":
                switch (tag)
                {
                    case "WeaponTrigger":
                        if (collision.tag == "CommonKnught" || collision.tag == "Hazardnut") collision.gameObject.GetComponent<DamageHandler>().HandleDamage(parent.GetComponent<Weapons>().currentWeapon.iDamage, parent.gameObject);
                        break;
                    case "GroundedTrigger":
                        switch (collision.tag)
                        {
                            case "Floor":
                                Debug.Log(parent.GetComponent<Rigidbody2D>().velocity.y);
                                if (parent.GetComponent<Rigidbody2D>().velocity.y <= 1) parent.GetComponent<playerController>().isGrounded = true;
                                break;
                            case "DeathBarrier":
                                gm.ReloadScene();
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
