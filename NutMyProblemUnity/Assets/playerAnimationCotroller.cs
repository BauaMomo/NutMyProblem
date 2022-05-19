using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimationCotroller : MonoBehaviour
{
    public enum State { idle, walking, running, airborne, crouching };
    [SerializeField] public State playerState;

    playerController playerController;
    Animator animator;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        playerState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isGrounded)
        {
            if (rb2D.velocity.x == 0) playerState = State.idle;
            else if (Input.GetKey(KeyCode.LeftShift)) playerState = State.running;
            else playerState = State.walking;
        }
        else playerState = State.airborne;

    }
}
