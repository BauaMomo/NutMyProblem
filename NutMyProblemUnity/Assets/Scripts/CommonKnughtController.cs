using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CommonKnughtController : MonoBehaviour
{
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
    float fStepBeginningTime;
    float fStandingBeginningTime;

    public int iSwordDamage;


    // Start is called before the first frame update
    void Start()
    {
        OnAttack = new UnityEvent();
        OnAttack.AddListener(GetComponent<CommonKnughtAnimationController>().OnAttack);

        fCommonKnughtSpeed = 4;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        CommonKnughtDirection = directions.right;
        mode = AIMode.patrol;

        iSwordDamage = 20;
        iAttackSpeed = 1;
        iRange = 1.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        CommonKnught = this.gameObject;

        fStepTime = 1;
        fStandingTime = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fCommonKnughtPathEndPoint, transform.position.y);
        startPosition = new Vector2(fCommonKnughtPathStartPoint, transform.position.y);

        EnemyMovement();
        TarggetPlayer();
        SwitchMovementMode();
        FlipEnemy();

        if (Vector3.Distance(transform.position, TPlayer.position) < 4.25f)
            StartCoroutine(SwordAttack(CommonKnughtDirection));
    }


    void EnemyMovement()
    {
        switch (mode)
        {
            case AIMode.patrol:
                if (transform.position.x < startPosition.x)
                {
                    CommonKnughtMoveDirection = new Vector3(6, 0, 0);
                    CommonKnughtDirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    CommonKnughtMoveDirection = new Vector3(-6, 0, 0);
                    CommonKnughtDirection = directions.left;
                    FlipEnemy();
                }

                if (transform.position.x >= startPosition.x && CommonKnughtDirection == directions.right)
                {
                    CommonKnughtMoveDirection = new Vector3(6, 0, 0);
                    if (transform.position.x >= endPosition.x)
                    {
                        CommonKnughtDirection = directions.left;
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && CommonKnughtDirection == directions.left)
                {
                    CommonKnughtMoveDirection = new Vector3(-6, 0, 0);
                    if (transform.position.x <= fCommonKnughtPathStartPoint)
                    {
                        CommonKnughtDirection = directions.right;
                        FlipEnemy();
                    }
                }


                switch (MoveStatus)
                {
                    case MoveState.stand:
                        //Invoke(nameof(EndStep), 0.1f);
                        break;

                    case MoveState.step:
                        //Invoke(nameof(StartStep), 0.25f);
                        break;
                }


                break;

            case AIMode.follow:
                rb.velocity = new Vector2(Mathf.Sign(Target.position.x - transform.position.x), 0) * fCommonKnughtSpeed;
                //rb.MovePosition(Vector2.MoveTowards(transform.position, Target.position, fCommonKnughtSpeed * Time.deltaTime));
                break;

        }

    }

    public void StartStep()
    {
        if (mode == AIMode.follow) return;

        rb.AddForce(CommonKnughtMoveDirection * 200);
        fStandingTime = Random.Range(fStandingTime - 0.5f, fStepTime + 0.5f);
    }

    public void EndStep()
    {
        if (mode == AIMode.follow) return;
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


        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            CommonKnughtDirection = directions.right;
            RayCastVector = new Vector3(5, 0);
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            CommonKnughtDirection = directions.left;
            RayCastVector = new Vector3(-5, 0);
        }

    }

    bool TarggetPlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + RayCastVector, 1 << LayerMask.NameToLayer("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Target = hit.collider.transform;
                return true;
            }
            else
            { return false; }
        }
        return false;
    }

    void SwitchMovementMode()
    {

        if (TarggetPlayer() == true)
        { mode = AIMode.follow; }

        if (Vector3.Distance(transform.position, TPlayer.transform.position) > 10)
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
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(5, 1);


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

