using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawbridgeController : MonoBehaviour
{
    GameObject BridgeRaised;
    GameObject BridgeLowered;

    bool isBridgeLowered = false;

    // Start is called before the first frame update
    void Start()
    {
        BridgeRaised = transform.Find("Drawbridge_Raised").gameObject;
        BridgeLowered = transform.Find("Drawbridge_Lowered").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WeaponTrigger")
        {
            //Debug.Log("trigger entered by " + collision.gameObject);

            Weapons weapons = collision.transform.parent.GetComponent<Weapons>();
            if (collision.transform.parent.tag == "Player" && weapons.currentWeapon.WeaponType == Weapons.Weapon.Type.Sword && !isBridgeLowered) LowerBridge();
        }
    }

    void LowerBridge()
    {
        isBridgeLowered = true;

        BridgeLowered.GetComponent<BoxCollider2D>().enabled = true;
        BridgeRaised.GetComponent<BoxCollider2D>().enabled = false;
    }
}
