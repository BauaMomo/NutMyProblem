using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HazardnutController : MonoBehaviour
{
    GameObject shadow;

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
    public Transform TPlayer;

    Vector2 endPosition;
    Vector2 startPosition;
    Vector3 RayCastVector;
    Vector2 WeaponDropPosition;

    public float fGlovesAttackSpeed { get; protected set; }
    public float fRange { get; protected set; }
    float lastAttackTime = -10f;
    float attackCooldown = 3f;
    [SerializeField] float fHazardnutPathStartPoint;
    [SerializeField] float fHazardnutPathEndPoint;
    float originalSpeed;
    float fHazardnutSpeed;
    float fHazardnutChargeSpeed;
    float fColliderSpawnTime;

    public int iGlovesDamage;

    [SerializeField] bool bHazardnutAwake;

    RaycastHit2D[] raycastArray = new RaycastHit2D[3];

    public bool battack;
    private bool noMovement;

    // Start is called before the first frame update
    void Start()
    {
        shadow = transform.Find("BlobShadow").gameObject;

        OnAttack = new UnityEvent();
        OnAttack.AddListener(GetComponent<HazardnutAnimationController>().OnAttack);

        originalSpeed = 6;
        fHazardnutSpeed = originalSpeed;
        fHazardnutChargeSpeed = 25;

        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        HazardnutDirection = directions.right;

        mode = AIMode.waiting;


        iGlovesDamage = 40;
        fGlovesAttackSpeed = 0.5f;
        fRange = 3.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        Hazardnut = this.gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateShadow();
    }

    private void FixedUpdate()
    {
        endPosition = new Vector2(fHazardnutPathEndPoint, transform.position.y);
        startPosition = new Vector2(fHazardnutPathStartPoint, transform.position.y);

        StartCoroutine(EnemyMovement());
        SwitchMovementMode();
        FlipEnemy();
    }

    IEnumerator EnemyMovement()
    {
        if (noMovement) yield break;

        switch (mode)
        {
            case AIMode.patrol:
                if (transform.position.x < startPosition.x)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, startPosition, fHazardnutSpeed * Time.deltaTime));
                    HazardnutDirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, endPosition, fHazardnutSpeed * Time.deltaTime));
                    HazardnutDirection = directions.left;
                    FlipEnemy();
                }

                if (transform.position.x >= startPosition.x && HazardnutDirection == directions.right)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, endPosition, fHazardnutSpeed * Time.deltaTime));
                    if (transform.position.x == endPosition.x)
                    {
                        HazardnutDirection = directions.left;
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && HazardnutDirection == directions.left)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, startPosition, fHazardnutSpeed * Time.deltaTime));
                    if (transform.position.x == fHazardnutPathStartPoint)
                    {
                        HazardnutDirection = directions.right;
                        FlipEnemy();
                    }
                }
                break;

            case AIMode.follow:

                if (transform.position.x - Target.position.x >= -4 && transform.position.x - Target.position.x <= 4 && transform.position.y == Target.position.y + -1)
                    rb.MovePosition(Vector2.MoveTowards(transform.position, Target.position, fHazardnutSpeed * Time.deltaTime));
                else
                {
                    if (transform.position.x < Target.position.x)
                        rb.MovePosition(Vector2.MoveTowards(transform.position, new Vector2(Target.position.x - 7, Target.position.y), fHazardnutSpeed * Time.deltaTime));
                    if (transform.position.x > Target.position.x)
                        rb.MovePosition(Vector2.MoveTowards(transform.position, new Vector2(Target.position.x + 7, Target.position.y), fHazardnutSpeed * Time.deltaTime));
                }

                break;

            case AIMode.waiting:

                break;

            case AIMode.attack:

                if (HazardnutDirection == directions.left)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, new Vector3(transform.position.x - 5, transform.position.y), (fHazardnutChargeSpeed) * Time.deltaTime));
                }
                if (HazardnutDirection == directions.right)
                {
                    rb.MovePosition(Vector2.MoveTowards(transform.position, new Vector3(transform.position.x + 5, transform.position.y), (fHazardnutChargeSpeed) * Time.deltaTime));
                }


                yield return new WaitForSeconds(0.6f);
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
                GetComponent<SpriteRenderer>().flipX = false;
                RayCastVector = new Vector3(10, 0);
                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = true;
                RayCastVector = new Vector3(-10, 0);
                break;
        }

        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            HazardnutDirection = directions.right;
            RayCastVector = new Vector3(10, 0);
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            HazardnutDirection = directions.left;
            RayCastVector = new Vector3(-10, 0);
        }
    }

    bool TargetPlayer()
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

                        if (Vector3.Distance(transform.position, TPlayer.position) < 10)
                            StartCoroutine(GlovesAttack(HazardnutDirection));
                        return true;
                    }

                    else
                    { return false; }

                }
                return false;

            case AIMode.waiting:

                Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(transform.position, 4);

                foreach (Collider2D collider in collider2Ds)
                {
                    if (collider.tag == "Player")
                    {
                        Target = collider.transform;
                        bHazardnutAwake = true;
                        return true;
                    }
                }

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
        if (TargetPlayer() == true && battack == false)
        {
            mode = AIMode.follow;
        }
        if (TargetPlayer() == false && bHazardnutAwake == false)
        { mode = AIMode.waiting; }


        /*{
            if (Vector3.Distance(transform.position, Target.position) > 10)
            { mode = AIMode.patrol; }
        }*/
    }

    public void DisableMovementFor(float _time)
    {
        if (mode == AIMode.attack) return;
        noMovement = true;
        Invoke(nameof(EnableMovement), _time);
    }

    void EnableMovement()
    {
        noMovement = false;
    }

    void UpdateShadow()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), new Vector2(0, -1), 10f, 1 << LayerMask.NameToLayer("Floor"));
        Debug.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, hit.point.y), Color.red);

        shadow.transform.position = new Vector2(transform.position.x, hit.point.y);                                     //Draws the shadow at the intersection of the ray and the next collider on the layer "Floor"
        if (hit.distance > 1f) shadow.transform.localScale = new Vector3(2, .6f, 1) * (0.5f / hit.distance + 0.5f);       //scales the shadow down with increasing distance from platform
        else shadow.transform.localScale = new Vector3(2, .6f, 1) * hit.distance;
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

    void ResetSpeed()
    {
        fHazardnutSpeed = originalSpeed;
    }

    public IEnumerator GlovesAttack(directions _directions)
    {
        if(noMovement) yield break;

        if (Time.time > lastAttackTime + attackCooldown)
        {
            OnAttack.Invoke();
            lastAttackTime = Time.time;

            yield return new WaitForSeconds(.5f);

            battack = true;
            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, Hazardnut.transform);
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(fRange, 1);

            float fColliderXOffset = 0.5f + fRange / 2;

            switch (_directions)
            {
                case directions.right:
                    break;
                case directions.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponTrigger.transform.position = Hazardnut.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);
            Destroy(weaponTrigger, 0.5f);

            fHazardnutSpeed = originalSpeed / 2;
            Invoke(nameof(ResetSpeed), 1.5f);
        }
    }

}
