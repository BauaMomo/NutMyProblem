using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommonKnughtController : MonoBehaviour
{
    GameObject shadow;

    public UnityEvent OnAttack;

    public enum directions { right, left };
    public directions CommonKnughtDirection;

    public enum Type { commonKnught, hazardnut }
    public Type EnemyType;

    enum AIMode { follow, patrol };
    AIMode mode;
    public enum MoveState { step, stand };
    public MoveState MoveStatus;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject CommonKnught;
    public GameObject WeaponDrop;
    GameObject weaponTrigger;

    GameManager gm;

    Rigidbody2D rb;

    Transform Target;
    public Transform TPlayer;

    public Vector2 goal;
    Vector2 endPosition;
    Vector2 startPosition;
    Vector2 WeaponDropPosition;
    Vector3 RayCastVector;
    Vector3 CommonKnughtMoveDirection;

    public float iAttackSpeed { get; protected set; }
    public float iRange { get; protected set; }
    [SerializeField] float fCommonKnughtPathStartPoint;
    [SerializeField] float fCommonKnughtPathEndPoint;
    float fCommonKnughtSpeed;
    float attackStartTime;
    float fStepTime;
    float fStandingTime;
    float patrolStandingTime = 2f;
    float followStandingTime = 0.7f;
    float fStepBeginningTime;
    float fStandingBeginningTime;

    public int iSwordDamage;
    bool noMovement;


    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.Find("BlobShadow").gameObject;

        OnAttack = new UnityEvent();
        OnAttack.AddListener(GetComponent<CommonKnughtAnimationController>().OnAttack);

        endPosition = new Vector2(fCommonKnughtPathEndPoint, transform.position.y);
        startPosition = new Vector2(fCommonKnughtPathStartPoint, transform.position.y);

        goal = startPosition;

        fCommonKnughtSpeed = 6;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        CommonKnughtDirection = directions.right;
        mode = AIMode.patrol;

        iSwordDamage = 20;
        iAttackSpeed = 1;
        iRange = 2f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        CommonKnught = this.gameObject;

        fStepTime = 1;
        fStandingTime = patrolStandingTime;
    }

    // Update is called once per frame
    void Update()
    {

        UpdateShadow();
        EnemyMovement();
        TarggetPlayer();
        SwitchMovementMode();
        FlipEnemy();

        if (TarggetPlayer())    //will only attack the player if they are in sight
            StartCoroutine(SwordAttack(CommonKnughtDirection));
    }


    void EnemyMovement()
    {
        if (noMovement) return;

        //if (rb.velocity.y < 0 && rb.velocity.y > -30) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 1.05f);

        switch (mode)
        {
            case AIMode.patrol:
                if (Vector2.Distance(transform.position, startPosition) < 1) goal = endPosition;        //CommonKnught decides which point is its goal and will move towards it
                if (Vector2.Distance(transform.position, endPosition) < 1) goal = startPosition;
                fStandingTime = patrolStandingTime;     //standingTime is adjusted for faster movement when following player
                break;

            case AIMode.follow:
                goal = Target.position;
                fStandingTime = followStandingTime;
                break;
        }

        if (transform.position.x < goal.x)
        {
            CommonKnughtMoveDirection = new Vector3(1, 0, 0) * fCommonKnughtSpeed;      //MoveDirection is set, now changes with speed
            CommonKnughtDirection = directions.right;
        }

        if (transform.position.x > goal.x)
        {
            CommonKnughtMoveDirection = new Vector3(1, 0, 0) * -fCommonKnughtSpeed;
            CommonKnughtDirection = directions.left;
        }
        FlipEnemy();       //only needs to be called once at the end
    }

    public void StartStep()     //StartStep and EndStep are called from the animation with animation events
    {
        if (noMovement) return;

        rb.AddForce(CommonKnughtMoveDirection * 100);
        fStandingTime = Random.Range(fStandingTime - 0.5f, fStepTime + 0.5f);
    }

    public void EndStep()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void FlipEnemy()
    {
        // Spriteflip im patrol mode
        switch (CommonKnughtDirection)
        {
            case directions.right:
                GetComponent<SpriteRenderer>().flipX = false;
                RayCastVector = new Vector3(5, 0);
                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = true;
                RayCastVector = new Vector3(-5, 0);
                break;
        }
    }

    bool TarggetPlayer()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, 2.5f);        //enemy now also sees the player if they are close behind it

        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.tag == "Player")
            {
                Target = collider.transform;
                return true;
            }
        }

        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + RayCastVector, 1 << LayerMask.NameToLayer("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Target = hit.collider.transform;
                return true;
            }
        }
        return false;
    }

    void SwitchMovementMode()
    {

        if (TarggetPlayer())
        { mode = AIMode.follow; }

        if (Vector3.Distance(transform.position, TPlayer.transform.position) > 20)
        { mode = AIMode.patrol; }

        if (MoveStatus == MoveState.step && Time.time > fStepBeginningTime + fStepTime)
        {
            fStandingBeginningTime = Time.time;
            MoveStatus = MoveState.stand;
        }

        if (MoveStatus == MoveState.stand && Time.time > fStandingBeginningTime + fStandingTime)
        {
            fStepBeginningTime = Time.time;
            MoveStatus = MoveState.step;
        }
    }
    void UpdateShadow()        //code for blob shadow
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 10f, 1 << LayerMask.NameToLayer("Floor"));
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, hit.point.y), Color.red);

        shadow.transform.position = new Vector2(transform.position.x, hit.point.y);                                     //Draws the shadow at the intersection of the ray and the next collider on the layer "Floor"
        if (hit.distance > 1f) shadow.transform.localScale = new Vector3(2, .6f, 1) * (0.5f / hit.distance + 0.5f);       //scales the shadow down with increasing distance from platform
        else shadow.transform.localScale = new Vector3(2, .6f, 1) * hit.distance;
    }

    public void DisableMovementFor(float _time)     //to stun enemies while they take knockback
    {
        noMovement = true;
        EndStep();
        Invoke(nameof(EnableMovement), _time);
    }

    void EnableMovement()
    {
        noMovement = false;
    }

    public void CommonKnughtDeath()
    {
        if (!gm.changingScene)
        {
            WeaponDropPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);
            WeaponDrop = Instantiate(Resources.Load("prefabs/WeaponDrop") as GameObject);
            WeaponDrop.transform.position = WeaponDropPosition;

            WeaponDrop.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-50f, 50f), 200));
            WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Sword);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator SwordAttack(directions _directions)
    {

        if (Time.time > attackStartTime + 2f)
        {

            attackStartTime = Time.time;
            OnAttack.Invoke();

            yield return new WaitForSeconds(0.5f);

            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, CommonKnught.transform);
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(7, 1);


            float fColliderXOffset = 0.5f + iRange / 2;

            switch (_directions)
            {
                case directions.right:
                    break;
                case directions.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponTrigger.transform.position = CommonKnught.transform.position + new Vector3(0, -0.1f, 0);
            Destroy(weaponTrigger, 0.2f);
        }
    }
}

