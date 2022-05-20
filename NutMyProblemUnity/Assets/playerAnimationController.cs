using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    public enum State { idle, walking, running, airborne, crouching, attacking };
    [SerializeField] public State playerState;

    public State currentAnimationState;

    public Dictionary<State, string> playerAnimations = new Dictionary<State, string>();

    playerController playerController;
    Animator animator;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        playerAnimations.Add(State.idle, "Player_Idle_Substitute_Animation");
        playerAnimations.Add(State.walking, "Player_Lauf_Ersatzanimation");
        playerAnimations.Add(State.running, "Player_Lauf_Ersatzanimation");
        playerAnimations.Add(State.airborne, "Player_Sprung_Ersatzanimation");
        playerAnimations.Add(State.attacking, "Player_Angriff_Ersatzanimation");
        playerAnimations.Add(State.crouching, "");

        playerState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        switchPlayerState();

        switchAnimation(playerState);

    }

    private void FixedUpdate()
    {
        
    }

    void switchPlayerState()
    {
        if (playerController.isGrounded)
        {
            if (rb2D.velocity.x == 0) playerState = State.idle;
            else if (Input.GetKey(KeyCode.LeftShift)) playerState = State.running;
            else playerState = State.walking;
        }
        else playerState = State.airborne;

        if (transform.Find("WeaponTrigger(Clone)") != null) playerState = State.attacking;
    }

    void switchAnimation(State _newState)
    {
        if (currentAnimationState == _newState) return;

        animator.Play(playerAnimations[_newState]);
        currentAnimationState = _newState;
    }
}

