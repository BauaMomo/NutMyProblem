using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawbridgeController : MonoBehaviour
{
    GameObject BridgeRaised;
    GameObject BridgeLowered;

    public bool isBridgeLowered = false;

    public ParticleSystem BridgeRopeCutParticle;

    // Start is called before the first frame update
    void Start()
    {
        BridgeRaised = transform.Find("Drawbridge_Raised").gameObject;
        BridgeLowered = transform.Find("Drawbridge_Lowered").gameObject;

        BridgeLowered.SetActive(false);
        BridgeRaised.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBridgeLowered)
        {
            BridgeRaised.SetActive(true);
            BridgeLowered.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WeaponTrigger")
        {
            //Debug.Log("trigger entered by " + collision.gameObject);
            if(isBridgeLowered == false)
            {
                BridgeRopeCutParticle.Play();
                StartCoroutine(StopParticle());
            }

            Weapons weapons = collision.transform.parent.GetComponent<Weapons>();
            if (collision.transform.parent.tag == "Player" && weapons.currentWeapon.WeaponType == Weapons.Weapon.Type.Sword && !isBridgeLowered)
            {
                LowerBridge();
                FindObjectOfType<AudioManager>().Play("RopeCut");
            }
            }
    }

    void LowerBridge()
    {
        isBridgeLowered = true;

        //BridgeLowered.GetComponent<BoxCollider2D>().enabled = true;
        // BridgeRaised.GetComponent<BoxCollider2D>().enabled = false;

        BridgeRaised.SetActive(false);
        BridgeLowered.SetActive(true);
    }
    IEnumerator StopParticle()
    {
        yield return new WaitForSeconds(0.5f);
        BridgeRopeCutParticle.Stop();
    }
}
