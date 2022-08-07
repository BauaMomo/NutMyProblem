using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWallController : MonoBehaviour
{
    bool isWallBroken = false;

    GameObject WallCracked;

    public ParticleSystem WallBrickParticle;
    public ParticleSystem WallSmokeParticle;

    // Start is called before the first frame update
    void Start()
    {
        WallCracked = transform.Find("BreakableWall_Cracked").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BreakWall()
    {
        isWallBroken = true;

        WallBrickParticle.Play();
        WallSmokeParticle.Play();

        WallCracked.GetComponent<SpriteRenderer>().enabled = false;
        WallCracked.GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Weapons weapons = collision.transform.parent.GetComponent<Weapons>();
        if (collision.transform.parent.tag == "Player" && weapons.currentWeapon.WeaponType == Weapons.Weapon.Type.Gloves && !isWallBroken)
        {
            BreakWall();
            StartCoroutine(StopParticle());
        }
    }
    IEnumerator StopParticle()
    {
        yield return new WaitForSeconds(0.5f);
        WallBrickParticle.Stop();
        WallSmokeParticle.Stop();
    }
}
