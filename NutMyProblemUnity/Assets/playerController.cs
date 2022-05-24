using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerController : MonoBehaviour
{
    Rigidbody2D rb;
    Weapons weapons;

    public enum State { idle, walking, running, crouching, airborne };
    public State playerState { get; protected set; }

    int iPlayerSpeed;
    [SerializeField] int iPlayerWalkSpeed;
    [SerializeField] int iPlayerSprintSpeed;
    [SerializeField] int iJumpSpeed;
    [SerializeField] int iFallSpeed;

    public bool isGrounded;
    bool leftRay;
    bool rightRay;
    public bool isSprinting { get; protected set; } = false;

    float fJumpStartTime;
    float fCollExitTime;

    float moveDir;
    public bool isHoldingJump { get; protected set; } = false;


    public enum direction { right, left };
    public direction playerDirection { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weapons = GetComponent<Weapons>();


        iPlayerWalkSpeed = 4;
        iPlayerSprintSpeed = 6;
        iJumpSpeed = 10;
        iFallSpeed = 13;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSprinting) iPlayerSpeed = iPlayerSprintSpeed;      //sprinting
        else iPlayerSpeed = iPlayerWalkSpeed;

        if (moveDir != 0) rb.velocity = new Vector2(iPlayerSpeed * moveDir, rb.velocity.y);

        if (rb.velocity.y < 0 && rb.velocity.y > -iFallSpeed)                                       //higher than standard fall speed
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 50 * Time.deltaTime);

        if (isHoldingJump && (Time.fixedUnscaledTime - fJumpStartTime) < 0.25) rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);    //higher jump when jump button is held
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<float>();
        if (moveDir > 0) playerDirection = direction.right;
        if (moveDir < 0) playerDirection = direction.left;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            //Debug.Log("jump start");
            isGrounded = false;
            isHoldingJump = true;
            fJumpStartTime = Time.fixedUnscaledTime;
            rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);
        }

        if (context.canceled) isHoldingJump = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log("mouse left pressed");
            weapons.currentWeapon.Attack(playerDirection);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started) isSprinting = true;
        if (context.canceled) isSprinting = false;
    }

    public void OnWeaponChange(InputAction.CallbackContext context)
    {
        if(context.started) weapons.SwitchWeapon();
    }

    bool IsOnPlatformEdge()
    {
        //Debug.Log("checking for edge");
        Vector3 pos = this.gameObject.transform.position;
        Debug.DrawLine(new Vector3(pos.x - 0.1f, pos.y, pos.z), new Vector3(pos.x - 0.1f, pos.y - 5f, pos.z));

        leftRay = Physics2D.Raycast(new Vector3(pos.x - 0.5f, pos.y, pos.z),        //casts two rays
            new Vector3(0, -1, 0), 3f);

        rightRay = Physics2D.Raycast(new Vector3(pos.x + 0.5f, pos.y, pos.z),
            new Vector3(0, -1, 0), 3f);

        return ((leftRay ^ rightRay) && isGrounded);     //if one of the rays hits a collider and the player is on the ground return true
        
    }
}
