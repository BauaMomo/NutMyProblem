using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HazardnutController : MonoBehaviour
{
    public enum directions { right, left };
    public directions HazardnutDirection;

    public enum Type { commonKnught, hazardnut }
    public Type EnemyType;

    enum AIMode { follow, patrol };
    AIMode mode;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject Hazardnut;
    public GameObject WeaponDrop;
    GameObject weaponTrigger;

    GameManager gm;
    Rigidbody2D rb;

    public UnityEvent OnAttack;

    Transform Target;
    [SerializeField] Transform CastPoint;
    public Transform TPlayer;

    Vector2 endPosition;
    Vector2 startPosition;
    Vector3 CastPointDirection;
    Vector2 WeaponDropPosition;

    public float fGlovesAttackSpeed { get; protected set; }
    public float iRange { get; protected set; }
    [SerializeField] float fHazardnutPathStartPoint;
    float fHazardnutPathEndPoint;
    float fHazardnutPathLength;
    float fHazardnutSpeed;
    float fColliderSpawnTime;

    public int iGlovesDamage;


    // Start is called before the first frame update
    void Start()
    {
        OnAttack = new UnityEvent();

        fHazardnutPathLength = 6;
        fHazardnutSpeed = 2;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        fHazardnutPathEndPoint = fHazardnutPathStartPoint + fHazardnutPathLength;
        HazardnutDirection = directions.right;
        mode = AIMode.patrol;


        iGlovesDamage = 30;
        fGlovesAttackSpeed = 0.5f;
        iRange = 1.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        Hazardnut = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fHazardnutPathEndPoint, transform.position.y);
        startPosition = new Vector2(fHazardnutPathStartPoint, transform.position.y);

        TarggetPlayer();
        EnemyMovement();
        SwitchMovementMode();
        FlipEnemy();

        if (Vector3.Distance(transform.position, TPlayer.position) < 8)
            GlovesAttack(HazardnutDirection);
    }

    void EnemyMovement()
    {
        switch (mode)
        {
            case AIMode.patrol:
                if (transform.position.x < startPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fHazardnutSpeed * Time.deltaTime);
                    HazardnutDirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fHazardnutSpeed * Time.deltaTime);
                    HazardnutDirection = directions.left;
                    FlipEnemy();
                }

                if (transform.position.x >= startPosition.x && HazardnutDirection == directions.right)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fHazardnutSpeed * Time.deltaTime);
                    if (transform.position.x == endPosition.x)
                    {
                        HazardnutDirection = directions.left;
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && HazardnutDirection == directions.left)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fHazardnutSpeed * Time.deltaTime);
                    if (transform.position.x == fHazardnutPathStartPoint)
                    {
                        HazardnutDirection = directions.right;
                        FlipEnemy();
                    }
                }
                break;

            case AIMode.follow:
                transform.position = Vector2.MoveTowards(transform.position, Target.position, fHazardnutSpeed * Time.deltaTime);
                break;
        }
    }

    void FlipEnemy()
    {
        // Spriteflip im patrol mode
        switch (HazardnutDirection)
        {
            case directions.right:
                GetComponent<SpriteRenderer>().flipX = false;
                CastPointDirection = CastPoint.position + Vector3.right * 10;
                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = true;
                CastPointDirection = CastPoint.position + -Vector3.right * 10;
                break;
        }

        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            HazardnutDirection = directions.right;
            CastPointDirection = CastPoint.position + Vector3.right * 5;
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            HazardnutDirection = directions.left;
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
            if (Vector3.Distance(transform.position, Target.position) > 10)
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
            WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Gloves);
        }
    }

    public void GlovesAttack(directions _directions)
    {
        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fGlovesAttackSpeed))
        {
            fColliderSpawnTime = Time.fixedUnscaledTime;

            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, Hazardnut.transform);
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
            weaponTrigger.transform.position = Hazardnut.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);

            Destroy(weaponTrigger, 0.1f);
        }
    }
}
