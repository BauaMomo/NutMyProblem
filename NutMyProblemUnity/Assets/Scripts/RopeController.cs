using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    public bool isRopeCut = false;
    bool isChandelierFalling = false;

    GameObject Chandelier;
    float chandelierFallStartTime;
    public bool isdown;
    bool playedSound;

    Animator animator;
    public GameObject Rope;

    public enum State { _default, cut };
    public State enemyState;

    Dictionary<State, string> RopeAnimations = new Dictionary<State, string>();

    // Start is called before the first frame update
    void Start()
    {
        Chandelier = transform.parent.Find("Chandelier").gameObject;
        playedSound = false;


        animator = GetComponent<Animator>();

        RopeAnimations.Add(State._default, "Rope_default_Animation");
        RopeAnimations.Add(State.cut, "Rope_Rip_Animation");
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
        isdown = hit;

        if (!hit)
        {
            Chandelier.transform.position = Vector2.MoveTowards(Chandelier.transform.position, Chandelier.transform.position + new Vector3(0, -5), 4f * chandelierAccel);
        }
        if (isdown == true && playedSound == false)
        {
            Chandelier.GetComponent<ChandelierManager>().ChangeSprite();
            FindObjectOfType<AudioManager>().Play("Kronleuchter");
            playedSound = true;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "WeaponTrigger")
        {
            //Debug.Log("trigger entered by " + collision.gameObject);

            Weapons weapons = collision.transform.parent.GetComponent<Weapons>();
            if (collision.transform.parent.tag == "Player" && weapons.currentWeapon.WeaponType == Weapons.Weapon.Type.Sword)
            {
                animator.Play(RopeAnimations[State.cut]);
                if (isRopeCut == false)
                {
                    FindObjectOfType<AudioManager>().Play("RopeCut");
                }
                isRopeCut = true;
            }
        }
    }



}
