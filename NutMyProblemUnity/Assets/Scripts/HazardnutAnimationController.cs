using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HazardnutAnimationController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    public UnityEvent OnDamaged;

    public enum State { idle, moving, attacking, stunned };
    public State enemyState;
    public State oldState;

    float attackStartTime;
    float attackTime = .8f;

    float stunStartTime;
    float stunTime = 1f;

    Dictionary<State, string> HazardnutAnimations = new Dictionary<State, string>();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        OnDamaged = new UnityEvent();
        OnDamaged.AddListener(StartStunPhase);

        HazardnutAnimations.Add(State.idle, "Hazardnut_Idle_Animation");
        HazardnutAnimations.Add(State.moving, "Hazardnut_Idle_Animation");
        HazardnutAnimations.Add(State.attacking, "Hazardnut_Attack_Animation");
        HazardnutAnimations.Add(State.stunned, "Hazardnut_Damaged_Animation");

        enemyState = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchState();
        SwitchAnimation(enemyState);
    }

    void SwitchAnimation(State newState)
    {
        if (newState == oldState) return;

        animator.Play(HazardnutAnimations[newState]);

        oldState = newState;
    }

    void SwitchState()
    {
        if (enemyState != State.attacking && enemyState != State.stunned)
        {
            if (rb.velocity.x == 0) enemyState = State.idle;
            else enemyState = State.moving;
        }

        if (enemyState == State.attacking && Time.time > attackStartTime + attackTime) enemyState = State.idle;

        if (enemyState == State.stunned && Time.time > stunStartTime + stunTime) enemyState = State.idle;
    }

    public void OnAttack()
    {
        Debug.Log("OnAttack called");
        attackStartTime = Time.time;
        enemyState = State.attacking;
    }
    void StartStunPhase()
    {
        stunStartTime = Time.time;
        enemyState = State.stunned;
    }
}
