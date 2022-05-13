using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Rigidbody2D rb;
    Weapons weapons;

    [SerializeField] int iPlayerWalkSpeed;
    [SerializeField] int iPlayerSprintSpeed;
    int iPlayerSpeed;
    [SerializeField] int iJumpSpeed;
    [SerializeField] public bool isGrounded;
    [SerializeField] int iFallSpeed;

    [SerializeField] bool leftRay;
    [SerializeField] bool rightRay;

    float fJumpStartTime;
    float fCollExitTime;

    public enum direction { right, left };
    direction playerDirection;


    /* TODO:
     * 
     * - different weapons
     *      (classes HandWeapon and ShootWeapon inherit from Weapon)
     * - Animation integration
     * 
     * 
     */

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
        if (Input.GetKey(KeyCode.LeftShift)) iPlayerSpeed = iPlayerSprintSpeed;      //sprinting
        else iPlayerSpeed = iPlayerWalkSpeed;

        if (Input.GetKey(KeyCode.D))                                                //walking
        {
            playerDirection = direction.right;
            if (rb.velocity.y < 0)
                rb.velocity = new Vector2(iPlayerSpeed / 1.3f, rb.velocity.y);
            else rb.velocity = new Vector2(iPlayerSpeed, rb.velocity.y);
        }

        if (Input.GetKey(KeyCode.A))
        {
            playerDirection = direction.left;
            if (rb.velocity.y < 0)
                rb.velocity = new Vector2(-iPlayerSpeed / 1.3f, rb.velocity.y);
            else rb.velocity = new Vector2(-iPlayerSpeed, rb.velocity.y);
        }


        if (Input.GetKey(KeyCode.Space) && isGrounded)              //jumps if player touches ground
        {
            //Debug.Log("jump start");
            isGrounded = false;
            fJumpStartTime = Time.fixedUnscaledTime;
            rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);
        }

        if (Input.GetKey(KeyCode.Space) && (Time.fixedUnscaledTime - fJumpStartTime) < 0.25f)           //jumps higher if space is held down
        {
            //Debug.Log(Time.fixedUnscaledTime - fJumpStartTime);
            rb.velocity = new Vector2(rb.velocity.x, iJumpSpeed);
        }

        if (rb.velocity.y < 0 && rb.velocity.y > -iFallSpeed)                                       //higher than standard fall speed
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 50 * Time.deltaTime);

        if (IsOnPlatformEdge() && !Input.anyKey) rb.velocity = new Vector2(0, rb.velocity.y);       //stops the player if they stand on a platform edge and no keys are pressed


        //if(Time.fixedUnscaledTime - fCollExitTime > 0.2f) isGrounded = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("mouse left pressed");
            weapons.Sword.Attack(playerDirection);
        }

        switch (playerDirection)
        {
            case direction.left:
                GetComponent<SpriteRenderer>().flipX = true;
                break;
            case direction.right:
                GetComponent<SpriteRenderer>().flipX = false;
                break;
        }
    }

    bool IsOnPlatformEdge()
    {
        //Debug.Log("checking for edge");
        Vector3 pos = this.gameObject.transform.position;

        leftRay = Physics2D.Raycast(new Vector3(pos.x - 0.1f, pos.y, pos.z),        //casts two rays
            new Vector3(0, -1, 0), 3f);

        rightRay = Physics2D.Raycast(new Vector3(pos.x + 0.1f, pos.y, pos.z),
            new Vector3(0, -1, 0), 3f);

        if ((leftRay ^ rightRay) && isGrounded) return true;                        //if either ray hits a collider and the player is on the ground return true
        else return false;
    }

    private void OnTriggerExit2D(Collider2D other)      
    {

    }


}
