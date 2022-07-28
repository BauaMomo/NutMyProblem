using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommonKnughtAnimationController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    CommonKnughtController controller;

    public UnityEvent OnDamaged;

    public enum State { idle, moving, attacking, stunned };
    public State enemyState;
    public State oldState;

    Vector2 oldPos = new Vector2();
    Vector2 newPos = new Vector2();
    bool moving;

    float attackStartTime;
    float attackTime = .8f;

    float stunStartTime;
    float stunTime = 1f;

    Dictionary<State, string> CommonKnughtAnimations = new Dictionary<State, string>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CommonKnughtController>();

        OnDamaged = new UnityEvent();
        OnDamaged.AddListener(StartStunPhase);

        CommonKnughtAnimations.Add(State.idle, "CommonKnught_Idle_Animation");
        CommonKnughtAnimations.Add(State.moving, "CommonKnught_Walk_Animation");
        CommonKnughtAnimations.Add(State.attacking, "CommonKnught_Attack_Animation");
        CommonKnughtAnimations.Add(State.stunned, "CommonKnught_Damaged_Animation");

        enemyState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchState();
        SwitchAnimation(enemyState);
    }

    private void FixedUpdate()
    {
        newPos = transform.position;
        moving = newPos != oldPos;
    }

    void SwitchAnimation(State newState)
    {
        if (newState == oldState) return;

        animator.Play(CommonKnughtAnimations[newState]);

        oldState = newState;
    }

    void SwitchState()
    {
        if (enemyState != State.attacking && enemyState != State.stunned)
        {
            if (controller.MoveStatus == CommonKnughtController.MoveState.stand) enemyState = State.idle;
            if (controller.MoveStatus == CommonKnughtController.MoveState.step) enemyState = State.moving;
        }

        if (enemyState == State.attacking && Time.time > attackStartTime + attackTime) enemyState = State.idle;

        if (enemyState == State.stunned && Time.time > stunStartTime + stunTime) enemyState = State.idle;
    }

    public void OnAttack()
    {
        //Debug.Log("OnAttack called");
        attackStartTime = Time.time;
        enemyState = State.attacking;
    }
    void StartStunPhase()
    {
        if (enemyState == State.attacking) return;
        stunStartTime = Time.time;
        enemyState = State.stunned;
    }
}