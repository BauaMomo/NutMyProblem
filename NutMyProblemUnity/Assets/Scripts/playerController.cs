using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class playerController : MonoBehaviour
{
    Rigidbody2D rb;
    Weapons weapons;
    playerAnimationController playerAnimationController;

    GameObject shadow;

    int iPlayerSpeed;
    [SerializeField] int iJumpSpeed;
    [SerializeField] int iFallSpeed;
    [SerializeField] int iFallAcceleration;

    public bool noMovement;
    float noMovementEndTime;

    public float lastDashTime { get; protected set; }
    public float fDashLength { get; protected set; } = .4f;
    public float fDashCooldown { get; } = .8f;
    float fDashStartHeight;

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
        playerAnimationController = GetComponent<playerAnimationController>();

        shadow = transform.Find("BlobShadow").gameObject;

        iPlayerSpeed = 12;
        iJumpSpeed = 18;
        iFallSpeed = 30;
        iFallAcceleration = 70;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsDashAvailable();

        if (noMovement && Time.time > noMovementEndTime) noMovement = false;

        if (!noMovement)
        {
            if (moveDir != 0) rb.velocity = new Vector2(iPlayerSpeed * moveDir, rb.velocity.y);
        }

        if (rb.velocity.y < 0 && rb.velocity.y > -iFallSpeed)                                       //higher than standard fall speed
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - iFallAcceleration * Time.deltaTime);

        if (isHoldingJump && (Time.fixedUnscaledTime - fJumpStartTime) < 0.25)
        {
            rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);    //higher jump if jump button is held down
        }

        if (playerAnimationController.playerState == playerAnimationController.State.dashing) rb.position = new Vector2( rb.position.x, fDashStartHeight );
    }

    private void Update()
    {
        UpdateShadow();
    }

    void UpdateShadow()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 10f, 1 << LayerMask.NameToLayer("Floor"));
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, hit.point.y), Color.red);

        shadow.transform.position = new Vector2(transform.position.x, hit.point.y);
        if (hit.distance > 1f) shadow.transform.localScale = new Vector3(2, 1, 1) * (0.5f / hit.distance + 0.5f);
        else shadow.transform.localScale = new Vector3(2, 1, 1) * hit.distance;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<float>();
        if (moveDir > 0) playerDirection = direction.right;
        if (moveDir < 0) playerDirection = direction.left;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !noMovement)
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

    public void OnWeaponChange(InputAction.CallbackContext context)
    {
        if (context.started) weapons.SwitchWeapon();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Collider2D[] searchColliders = Physics2D.OverlapCircleAll(this.transform.position, 2);      //creates an array of colliders within a certain radius

            List<GameObject> WeaponDropObjects = new List<GameObject>();
            List<float> WeaponDropDistances = new List<float>();

            foreach (Collider2D collider in searchColliders)        //adds all WeaponDrop GameObjects to their List
            {
                if (collider.gameObject.tag == "WeaponDrop")
                {
                    WeaponDropObjects.Add(collider.gameObject);
                    WeaponDropDistances.Add(Vector3.Distance(this.transform.position, collider.transform.position));
                }
            }

            GameObject DropToPickUp = WeaponDropObjects.Find(x => Vector3.Distance(x.transform.position, this.transform.position) == WeaponDropDistances.Min());    //Finds the WeaponDrop with the smallest distance

            weapons.AddWeaponFromDrop(DropToPickUp);
            
            Destroy(DropToPickUp);

        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && IsDashAvailable())
        {
            lastDashTime = Time.time;

            noMovementEndTime = Time.time + fDashLength;
            noMovement = true;

            fDashStartHeight = rb.position.y;

            rb.AddForce(new Vector2(1000, 0) * moveDir);
        }
    }

    public bool IsDashAvailable()
    {
        return Time.time > lastDashTime + fDashCooldown;
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
