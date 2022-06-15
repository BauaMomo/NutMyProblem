using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class playerAnimationController : MonoBehaviour
{
    float attackAnimationStartTime;
    float currentWeaponAttackLength;
    public enum State { idle, walking, running, airborne, crouching, attacking };
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

        SwordAnimations.Add(State.idle, "Player_Idle_Substitute_Animation");
        SwordAnimations.Add(State.walking, "Player_Lauf_Ersatzanimation");
        SwordAnimations.Add(State.running, "Player_Lauf_Ersatzanimation");
        SwordAnimations.Add(State.airborne, "Player_Sprung_Ersatzanimation");
        SwordAnimations.Add(State.attacking, "Player_Angriff_Ersatzanimation");
        SwordAnimations.Add(State.crouching, "");

        GloveAnimations.Add(State.idle, "Player_Gloves_Idle_Animation");
        GloveAnimations.Add(State.walking, "Player_Gloves_Run_Substitute_Animation");
        GloveAnimations.Add(State.running, "Player_Gloves_Run_Substitute_Animation");
        GloveAnimations.Add(State.airborne, "Player_Gloves_Jump_Substitute_Animation");
        GloveAnimations.Add(State.attacking, "Player_Gloves_Attack_Substitute_Animation");
        GloveAnimations.Add(State.crouching, "");

        BowAnimations.Add(State.idle, "");
        BowAnimations.Add(State.walking, "");
        BowAnimations.Add(State.running, "");
        BowAnimations.Add(State.airborne, "");
        BowAnimations.Add(State.attacking, "");
        BowAnimations.Add(State.crouching, "");

        FistAnimations.Add(State.idle, "Player_WithoutWeapon_Idle_Animation");
        FistAnimations.Add(State.walking, "Player_WithoutWeapon_Run_Animation");
        FistAnimations.Add(State.running, "Player_WithoutWeapon_Run_Animation");
        FistAnimations.Add(State.airborne, "Player_WithoutWeapon_Jump_Animation");
        FistAnimations.Add(State.attacking, "Player_WithoutWeapon_Attack_Animation");
        FistAnimations.Add(State.crouching, "");

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

        oldWeaponType = weapons.currentWeapon.WeaponType;
    }

    private void FixedUpdate()
    {
        switchPlayerState();
    }

    void switchPlayerState()
    {
        if (attackAnimationStartTime + currentWeaponAttackLength > Time.fixedUnscaledTime)
        {
            playerState = State.attacking;
            return;
        }

        if (playerController.isGrounded)
        {
            if (rb2D.velocity.x == 0) playerState = State.idle;
            else if (playerController.isSprinting) playerState = State.running;
            else playerState = State.walking;
        }
        else playerState = State.airborne;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackAnimationStartTime = Time.fixedUnscaledTime;
        }
    }

    void switchAnimation(State _newState)
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

