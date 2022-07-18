using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonKnughtController : MonoBehaviour
{
    public enum directions { right, left };
    public directions CommonKnughtDirection;

    public enum Type { commonKnught, hazardnut }
    public Type EnemyType;

    enum AIMode { follow, patrol };
    AIMode mode;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject CommonKnught;
    public GameObject WeaponDrop;
    GameObject weaponTrigger;

    GameManager gm;

    Rigidbody2D rb;

    Transform Target;
    [SerializeField] Transform CastPoint;
    public Transform TPlayer;

    Vector2 endPosition;
    Vector2 startPosition;
    Vector3 CastPointDirection;
    Vector2 WeaponDropPosition;

    public float iAttackSpeed { get; protected set; }
    public float iRange { get; protected set; }
    [SerializeField] float fCommonKnughtPathStartPoint;
    float fCommonKnughtPathEndPoint;
    float fCommonKnughtPathLength;
    float fCommonKnughtSpeed;
    float fColliderSpawnTime;

    public int iSwordDamage;


    // Start is called before the first frame update
    void Start()
    {
        fCommonKnughtPathLength = 6;
        fCommonKnughtSpeed = 2;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        fCommonKnughtPathEndPoint = fCommonKnughtPathStartPoint + fCommonKnughtPathLength;
        CommonKnughtDirection = directions.right;
        mode = AIMode.patrol;

        iSwordDamage = 20;
        iAttackSpeed = 1;
        iRange = 1.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        CommonKnught = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fCommonKnughtPathEndPoint, transform.position.y);
        startPosition = new Vector2(fCommonKnughtPathStartPoint, transform.position.y);

        TarggetPlayer();
        EnemyMovement();
        SwitchMovementMode();
        FlipEnemy();

        if (Vector3.Distance(transform.position, TPlayer.position) < 1)
            SwordAttack(CommonKnughtDirection);
    }


    void EnemyMovement()
    {
        switch (mode)
        {
            case AIMode.patrol:
                if (transform.position.x < startPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fCommonKnughtSpeed * Time.deltaTime);
                    CommonKnughtDirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fCommonKnughtSpeed * Time.deltaTime);
                    CommonKnughtDirection = directions.left;
                    FlipEnemy();
                }

                if (transform.position.x >= startPosition.x && CommonKnughtDirection == directions.right)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fCommonKnughtSpeed * Time.deltaTime);
                    if (transform.position.x == endPosition.x)
                    {
                        CommonKnughtDirection = directions.left;
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && CommonKnughtDirection == directions.left)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fCommonKnughtSpeed * Time.deltaTime);
                    if (transform.position.x == fCommonKnughtPathStartPoint)
                    {
                        CommonKnughtDirection = directions.right;
                        FlipEnemy();
                    }
                }
                break;

            case AIMode.follow:
                transform.position = Vector2.MoveTowards(transform.position, Target.position, fCommonKnughtSpeed * Time.deltaTime);
                break;
        }

    }

    void FlipEnemy()
    {
        // Spriteflip im patrol mode
        switch (CommonKnughtDirection)
        {
            case directions.right:
                GetComponent<SpriteRenderer>().flipX = false;
                CastPointDirection = CastPoint.position + Vector3.right * 5;
                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = true;
                CastPointDirection = CastPoint.position + -Vector3.right * 5;
                break;
        }


        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            CommonKnughtDirection = directions.right;
            CastPointDirection = CastPoint.position + Vector3.right * 5;
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            CommonKnughtDirection = directions.left;
            CastPointDirection = CastPoint.position + -Vector3.right * 5;
        }

    }

    bool TarggetPlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(CastPoint.position, CastPointDirection, 1 << LayerMask.NameToLayer("Player"));

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
        else
        {
            if (Vector3.Distance(transform.position, Target.position) > 5)
            { mode = AIMode.patrol; }
        }
    }
    private void OnDestroy()
    {
        if (!gm.changingScene)
        {
            WeaponDropPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);
            WeaponDrop = Instantiate(Resources.Load("prefabs/WeaponDrop") as GameObject);
            WeaponDrop.transform.position = WeaponDropPosition;

            WeaponDrop.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-50f, 50f), 200));
            WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Sword);
        }

    }



    public void SwordAttack(directions _directions)
    {

        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / iAttackSpeed))
        {
            fColliderSpawnTime = Time.fixedUnscaledTime;

            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, CommonKnught.transform);
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(iRange, 1);

            float fColliderXOffset = 0.5f + iRange / 2;

            switch (_directions)
            {
                case directions.right:
                    break;
                case directions.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponTrigger.transform.position = CommonKnught.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);

            Destroy(weaponTrigger, 0.1f);
        }
    }
}

