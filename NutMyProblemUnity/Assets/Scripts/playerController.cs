using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class playerController : MonoBehaviour
{
    Rigidbody2D rb;
    GameManager gm;
    Weapons weapons;
    playerAnimationController playerAnimationController;

    GameObject shadow;
    GameObject DeathBarrier;
    Camera PlayerCamera;

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
    float defaultGravity;
    float dashGravity = 1;
    bool isDashing;
    bool hasDash;

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
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = Object.FindObjectOfType<GameManager>();
        weapons = GetComponent<Weapons>();
        playerAnimationController = GetComponent<playerAnimationController>();

        defaultGravity = rb.gravityScale;

        shadow = transform.Find("BlobShadow").gameObject;
        DeathBarrier = Instantiate(Resources.Load("Prefabs/DeathBarrier") as GameObject);
        DeathBarrier.transform.position = transform.position;

        PlayerCamera = Camera.main;
        PlayerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        iPlayerSpeed = 12;
        iJumpSpeed = 16;
        iFallSpeed = 30;
        iFallAcceleration = 70;
    }

    private void Update()
    {
        if (isGrounded) hasDash = true;

        MovePlayer();
        UpdateShadow();
        MoveDeathBarrier();
    }

    void MovePlayer()
    {
        rb.gravityScale = defaultGravity;

        IsDashAvailable();
        if (Time.time > lastDashTime + fDashLength) isDashing = false;
        if (isDashing) rb.gravityScale = dashGravity;

        if (!noMovement)
        {
            if (moveDir != 0) rb.velocity = new Vector2(iPlayerSpeed * moveDir, rb.velocity.y);
        }
        else if(Time.time > noMovementEndTime) noMovement = false;

        if (!isDashing)
        {
            if (rb.velocity.y < 0 && rb.velocity.y > -iFallSpeed)                                                   //higher than standard fall speed
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - iFallAcceleration * Time.deltaTime);

            if (isHoldingJump && (Time.fixedUnscaledTime - fJumpStartTime) < 0.25)                                  //higher jump if jump button is held down
            {
                rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);
            }
        }

        
    }

    void UpdateShadow()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 10f, 1 << LayerMask.NameToLayer("Floor"));
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, hit.point.y), Color.red);

        shadow.transform.position = new Vector2(transform.position.x, hit.point.y);                                     //Draws the shadow at the intersection of the ray and the next collider on the layer "Floor"
        if (hit.distance > 1f) shadow.transform.localScale = new Vector3(2, 1, 1) * (0.5f / hit.distance + 0.5f);       //scales the shadow down with increasing distance from platform
        else shadow.transform.localScale = new Vector3(2, 1, 1) * hit.distance;
    }

    void MoveDeathBarrier()
    {
        DeathBarrier.transform.position = new Vector2(transform.position.x, -20);
    }

    // v Unity InputSystem Stuff v

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
            List<GameObject> AllGO = new List<GameObject>();

            foreach (Collider2D c in searchColliders)
            {
                AllGO.Add(c.gameObject);
            }

            List<GameObject> SortedGos = SortGOByDistance(AllGO, transform);

            foreach (GameObject go in SortedGos)
            {
                switch (go.tag)
                {
                    case "WeaponDrop":
                        PickUpDrop(go);
                        return;
                    case "Lever":
                        go.GetComponent<LeverController>().SwitchLever();
                        return;
                }
            }
        }
    }

    void PickUpDrop(GameObject _drop)
    {
        weapons.AddWeaponFromDrop(_drop);

        Destroy(_drop);
    }

    List<GameObject> SortGOByDistance(List<GameObject> _gameObjects, Transform _target)
    {
        List<float> GODistances = new List<float>();
        foreach (GameObject go in _gameObjects)
        {
            GODistances.Add(Vector2.Distance(go.transform.position, _target.position));
        }

        List<GameObject> SortedGOs = new List<GameObject>();
        foreach (GameObject go in _gameObjects)
        {
            int addIndex = GODistances.IndexOf(GODistances.Min());
            GODistances[addIndex] = float.MaxValue;
            SortedGOs.Add(_gameObjects[addIndex]);
        }

        return SortedGOs;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && IsDashAvailable())
        {
            isDashing = true;
            lastDashTime = Time.time;

            noMovementEndTime = Time.time + fDashLength;
            noMovement = true;


            rb.AddForce(new Vector2(1000, 0) * moveDir);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (!isGrounded) hasDash = false;
    }

    public bool IsDashAvailable()
    {
        if(!hasDash) return false;
        if(moveDir == 0) return false;
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
