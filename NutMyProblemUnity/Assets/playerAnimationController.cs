using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    public enum State { idle, walking, running, airborne, crouching, attacking };
    [SerializeField] public State playerState;

    public State currentAnimationState;

    public Dictionary<State, string> SwordAnimations = new Dictionary<State, string>();
    public Dictionary<State, string> GloveAnimations = new Dictionary<State, string>();
    public Dictionary<State, string> BowAnimations = new Dictionary<State, string>();

    playerController playerController;
    Animator animator;
    Rigidbody2D rb2D;
    Weapons weapons;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        weapons = GetComponent<Weapons>();

        SwordAnimations.Add(State.idle, "Player_Idle_Substitute_Animation");
        SwordAnimations.Add(State.walking, "Player_Lauf_Ersatzanimation");
        SwordAnimations.Add(State.running, "Player_Lauf_Ersatzanimation");
        SwordAnimations.Add(State.airborne, "Player_Sprung_Ersatzanimation");
        SwordAnimations.Add(State.attacking, "Player_Angriff_Ersatzanimation");
        SwordAnimations.Add(State.crouching, "");

        GloveAnimations.Add(State.idle, "");
        GloveAnimations.Add(State.walking, "");
        GloveAnimations.Add(State.running, "");
        GloveAnimations.Add(State.airborne, "");
        GloveAnimations.Add(State.attacking, "");
        GloveAnimations.Add(State.crouching, "");

        BowAnimations.Add(State.idle, "");
        BowAnimations.Add(State.walking, "");
        BowAnimations.Add(State.running, "");
        BowAnimations.Add(State.airborne, "");
        BowAnimations.Add(State.attacking, "");
        BowAnimations.Add(State.crouching, "");

        playerState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {

        switchAnimation(playerState);

        switch (playerController.playerDirection)
        {
            case playerController.direction.left:
                GetComponent<SpriteRenderer>().flipX = true;
                break;
            case playerController.direction.right:
                GetComponent<SpriteRenderer>().flipX = false;
                break;
        }

    }

    private void FixedUpdate()
    {
        switchPlayerState();
    }

    void switchPlayerState()
    {
        if (playerController.isGrounded)
        {
            if (rb2D.velocity.x == 0) playerState = State.idle;
            else if (playerController.isSprinting) playerState = State.running;
            else playerState = State.walking;
        }
        else playerState = State.airborne;

        if (transform.Find("WeaponTrigger(Clone)") != null) playerState = State.attacking;


    }

    void switchAnimation(State _newState)
    {
        if (currentAnimationState == _newState) return;

        Weapons.Weapon currentWeapon = weapons.currentWeapon;

        switch (currentWeapon.WeaponType)
        {
            case Weapons.Weapon.Type.Sword:
                animator.Play(SwordAnimations[_newState]);
                break;
            case Weapons.Weapon.Type.Gloves:
                animator.Play(GloveAnimations[_newState]);
                break;
            case Weapons.Weapon.Type.Bow:
                animator.Play(BowAnimations[_newState]);
                break;
        }
        currentAnimationState = _newState;
    }
}

