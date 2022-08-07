using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public bool isRopeCut = false;
    bool isChandelierFalling = false;

    GameObject Chandelier;
    float chandelierFallStartTime;

    public ParticleSystem RopeCutParticle;

    // Start is called before the first frame update
    void Start()
    {
        Chandelier = transform.parent.Find("Chandelier").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRopeCut) StartFall();
    }

    void StartFall()
    {
        if (!isChandelierFalling) chandelierFallStartTime = Time.time;
        isChandelierFalling = true;

        float chandelierAccel = (Time.time - chandelierFallStartTime) / 10;


        RaycastHit2D hit = Physics2D.Raycast(Chandelier.transform.position - new Vector3(0, 3f), new Vector2(0, -1), 2f, 1 << LayerMask.NameToLayer("Floor"));

        if (!hit)
        {
            Chandelier.transform.position = Vector2.MoveTowards(Chandelier.transform.position, Chandelier.transform.position + new Vector3(0, -5), 4f * chandelierAccel);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "WeaponTrigger")
        {
            //Debug.Log("trigger entered by " + collision.gameObject);

            RopeCutParticle.Play();
            StartCoroutine(StopParticle());

            Weapons weapons = collision.transform.parent.GetComponent<Weapons>();
            if (collision.transform.parent.tag == "Player" && weapons.currentWeapon.WeaponType == Weapons.Weapon.Type.Sword)    isRopeCut = true;
        }
    }
    IEnumerator StopParticle()
    {
        yield return new WaitForSeconds(0.5f);
        RopeCutParticle.Stop();
    }
}
