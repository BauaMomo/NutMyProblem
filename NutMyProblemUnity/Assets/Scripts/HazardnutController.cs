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

    enum AIMode { follow, patrol, waiting, attack };
    [SerializeField] AIMode mode;

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
    Vector3 RayCastVector;
    Vector2 WeaponDropPosition;

    public float fGlovesAttackSpeed { get; protected set; }
    public float iRange { get; protected set; }
    [SerializeField] float fHazardnutPathStartPoint;
    [SerializeField] float fHazardnutPathEndPoint;
    float fHazardnutSpeed;
    float fHazardnutChargeSpeed;
    float fColliderSpawnTime;

    public int iGlovesDamage;

    [SerializeField] bool bHazardnutAwake;

    RaycastHit2D[] raycastArray = new RaycastHit2D[3];

    public bool battack;

    // Start is called before the first frame update
    void Start()
    {
        OnAttack = new UnityEvent();
        OnAttack.AddListener(GetComponent<HazardnutAnimationController>().OnAttack);

        fHazardnutSpeed = 4;
        fHazardnutChargeSpeed = 20;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        HazardnutDirection = directions.right;

        mode = AIMode.waiting;


        iGlovesDamage = 30;
        fGlovesAttackSpeed = 0.5f;
        iRange = 3.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        Hazardnut = this.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fHazardnutPathEndPoint, transform.position.y);
        startPosition = new Vector2(fHazardnutPathStartPoint, transform.position.y);

        StartCoroutine(EnemyMovement());
        SwitchMovementMode();
        FlipEnemy();

    }

    IEnumerator EnemyMovement()
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

                if (transform.position.x - Target.position.x >= -4 && transform.position.x - Target.position.x <= 4 && transform.position.y == Target.position.y + -1)
                    transform.position = Vector2.MoveTowards(transform.position, Target.position, fHazardnutSpeed * Time.deltaTime);
                else
                {
                    if (transform.position.x < Target.position.x)
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(Target.position.x - 4, Target.position.y), fHazardnutSpeed * Time.deltaTime);
                    if (transform.position.x > Target.position.x)
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(Target.position.x + 4, Target.position.y), fHazardnutSpeed * Time.deltaTime);
                }

                break;

            case AIMode.waiting:

                break;

            case AIMode.attack:

                if (HazardnutDirection == directions.left)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x - 5, transform.position.y), (fHazardnutChargeSpeed) * Time.deltaTime);
                }
                if (HazardnutDirection == directions.right)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector3(transform.position.x + 5, transform.position.y), (fHazardnutChargeSpeed) * Time.deltaTime);
                }


                yield return new WaitForSeconds(0.8f);
                battack = false;
                mode = AIMode.follow;
                break;
        }
    }

    void FlipEnemy()
    {
        // Spriteflip im patrol mode
        switch (HazardnutDirection)
        {
            case directions.right:
                GetComponent<SpriteRenderer>().flipX = true;
                RayCastVector = new Vector3(10, 0);
                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = false;
                RayCastVector = new Vector3(-10, 0);
                break;
        }

        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            HazardnutDirection = directions.right;
            RayCastVector = new Vector3(5, 0);
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            HazardnutDirection = directions.left;
            RayCastVector = new Vector3(-5, 0);
        }
    }

    bool TarggetPlayer()
    {
        switch (mode)
        {
            case AIMode.follow:
                RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + RayCastVector, 1 << LayerMask.NameToLayer("Player"));
                if (hit.collider != null)
                {

                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        Target = hit.collider.transform;

                        if (Vector3.Distance(transform.position, TPlayer.position) < 4.5f)
                            StartCoroutine(GlovesAttack(HazardnutDirection));
                        return true;
                    }

                    else
                    { return false; }

                }
                return false;

            case AIMode.waiting:

                raycastArray[0] = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -5), 1 << LayerMask.NameToLayer("Player"));
                raycastArray[1] = Physics2D.Linecast(transform.position, transform.position + new Vector3(5, -5), 1 << LayerMask.NameToLayer("Player"));
                raycastArray[2] = Physics2D.Linecast(transform.position, transform.position + new Vector3(-5, -5), 1 << LayerMask.NameToLayer("Player"));

                foreach (RaycastHit2D Hitdirection in raycastArray)
                {
                    if (Hitdirection.collider != null)
                    {
                        if (Hitdirection.collider.gameObject.CompareTag("Player"))
                        {
                            Target = Hitdirection.collider.transform;
                            bHazardnutAwake = true;
                            return true;
                        }

                        else
                        { return false; }

                    }
                }

                return false;
        }
        return false;

    }

    void SwitchMovementMode()
    {
        if (battack == true)
        {
            mode = AIMode.attack;

        }
        if (TarggetPlayer() == true && battack == false)
        {
            mode = AIMode.follow;
        }
        if (TarggetPlayer() == false && bHazardnutAwake == false)
        { mode = AIMode.waiting; }


        /*{
            if (Vector3.Distance(transform.position, Target.position) > 10)
            { mode = AIMode.patrol; }
        }*/
    }

    public void HazardnutDeath()
    {
        if (!gm.changingScene)
        {
            WeaponDropPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);
            WeaponDrop = Instantiate(Resources.Load("prefabs/WeaponDrop") as GameObject);
            WeaponDrop.transform.position = WeaponDropPosition;

            WeaponDrop.GetComponent<Rigidbody2D>().AddForce(new Vector2(UnityEngine.Random.Range(-50f, 50f), 200));
            WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Gloves);
            Destroy(this.gameObject);
        }
    }




    public IEnumerator GlovesAttack(directions _directions)
    {

        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fGlovesAttackSpeed))
        {
            OnAttack.Invoke();
            fColliderSpawnTime = Time.fixedUnscaledTime;

            yield return new WaitForSeconds(0.5f);

            battack = true;
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
