using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class playerAnimationController : MonoBehaviour
{
    float attackAnimationStartTime;
    float currentWeaponAttackLength;
    public enum State { idle, walking, airborne, crouching, attacking, dashing };
    [SerializeField] public State playerState;

    public State currentAnimationState;

    public Dictionary<State, string> SwordAnimations = new Dictionary<State, string>();     //dictionaries to translate from playerState to animation clip name
    public Dictionary<State, string> GloveAnimations = new Dictionary<State, string>();
    public Dictionary<State, string> BowAnimations = new Dictionary<State, string>();
    public Dictionary<State, string> FistAnimations = new Dictionary<State, string>();

    playerController playerController;
    Animator animator;
    Rigidbody2D rb2D;
    Weapons weapons;

    Weapons.Weapon.Type oldWeaponType;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        weapons = GetComponent<Weapons>();

        SwordAnimations.Add(State.idle, "Player_Sword_Idle_Animation");
        SwordAnimations.Add(State.walking, "Player_Sword_Run_Animation");
        SwordAnimations.Add(State.airborne, "Player_Sword_Jump_Animation");
        SwordAnimations.Add(State.attacking, "Player_Sword_Attack_Animation");
        SwordAnimations.Add(State.crouching, "");
        SwordAnimations.Add(State.dashing, "Player_Sword_Dash_Animation");

        GloveAnimations.Add(State.idle, "Player_Gloves_Idle_Animation");
        GloveAnimations.Add(State.walking, "Player_Gloves_Run_Animation");
        GloveAnimations.Add(State.airborne, "Player_Gloves_Jump_Animation");
        GloveAnimations.Add(State.attacking, "Player_Gloves_Attack_Animation");
        GloveAnimations.Add(State.crouching, "");
        GloveAnimations.Add(State.dashing, "Player_Gloves_Dash_Animation");

        BowAnimations.Add(State.idle, "");
        BowAnimations.Add(State.walking, "");
        BowAnimations.Add(State.airborne, "");
        BowAnimations.Add(State.attacking, "");
        BowAnimations.Add(State.crouching, "");
        BowAnimations.Add(State.dashing, "");

        FistAnimations.Add(State.idle, "Player_WithoutWeapon_Idle_Animation");
        FistAnimations.Add(State.walking, "Player_WithoutWeapon_Run_Animation");
        FistAnimations.Add(State.airborne, "Player_WithoutWeapon_Jump_Animation");
        FistAnimations.Add(State.attacking, "Player_WithoutWeapon_Attack_Animation");
        FistAnimations.Add(State.crouching, "");
        FistAnimations.Add(State.dashing, "Player_WithoutWeapon_Dash_Animation");

        playerState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimation(playerState);
        if (playerState != State.attacking) FlipSprite();

        oldWeaponType = weapons.currentWeapon.WeaponType;
    }

    void FlipSprite()
    {
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
        SwitchPlayerState();
    }

    void SwitchPlayerState()
    {
        if (attackAnimationStartTime + currentWeaponAttackLength > Time.fixedUnscaledTime)
        {
            playerState = State.attacking;
            return;
        }

        if (Time.time < playerController.lastDashTime + playerController.fDashLength)
        {
            playerState = State.dashing;
            return;
        }

        if (playerController.isGrounded)
        {
            if (rb2D.velocity.x == 0) playerState = State.idle;
            else playerState = State.walking;
        }
        else playerState = State.airborne;
    }

    public void OnAttack()
    {
            attackAnimationStartTime = Time.fixedUnscaledTime;
    }

    void SwitchAnimation(State _newState)
    {
        Weapons.Weapon currentWeapon = weapons.currentWeapon;

        if (currentAnimationState == _newState
            && !WeaponHasChanged(currentWeapon.WeaponType)) return;

        switch (currentWeapon.WeaponType)
        {
            case Weapons.Weapon.Type.Sword:
                animator.Play(SwordAnimations[_newState]);
                currentWeaponAttackLength = GetClipLength(SwordAnimations[State.attacking]);
                break;
            case Weapons.Weapon.Type.Gloves:
                animator.Play(GloveAnimations[_newState]);
                currentWeaponAttackLength = GetClipLength(GloveAnimations[State.attacking]);
                break;
            case Weapons.Weapon.Type.Fists:
                animator.Play(FistAnimations[_newState]);
                currentWeaponAttackLength = GetClipLength(FistAnimations[State.attacking]);
                break;
            case Weapons.Weapon.Type.Bow:
                animator.Play(BowAnimations[_newState]);
                currentWeaponAttackLength = GetClipLength(BowAnimations[State.attacking]);
                break;
        }
        if (currentWeaponAttackLength < 0.2f) currentWeaponAttackLength = 0.4f; //for substitute animations
        currentAnimationState = _newState;
    }

    float GetClipLength(string _clipName)
    {
        return animator.runtimeAnimatorController.animationClips.ToList<AnimationClip>().Find(clip => clip.name == _clipName).length;
    }

    bool WeaponHasChanged(Weapons.Weapon.Type _newWeaponType)
    {
        return !(_newWeaponType == oldWeaponType);
    }
}

