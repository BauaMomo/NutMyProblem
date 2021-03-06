using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum directions { right, left };
    enum AIMode { follow, patrol };

    public directions EnemyDirection;

    [SerializeField] public GameObject WeaponDrop;

    [SerializeField] AIMode mode;

    GameManager gm;

    Rigidbody2D rb;
    Transform Target;
    [SerializeField] Transform CastPoint;

    Vector2 endPosition;
    Vector2 startPosition;
    Vector3 CastPointDirection;
    Vector2 WeaponDropPosition;


    [SerializeField] float fEnemyPathStartPoint;
    float fEnemyPathEndPoint;
    float fEnemyPathLength;
    float fEnemySpeed;

    // Start is called before the first frame update
    void Start()
    {
        //kann sp?ter raus genommen werden und in Unity bestimmt werden -> maybe??!
        // PathStartPoint ist der Spawnpoint des Gegners
        //fEnemyPathStartPoint = 4;
        fEnemyPathLength = 6;
        fEnemySpeed = 2;


        gm = Object.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        fEnemyPathEndPoint = fEnemyPathStartPoint + fEnemyPathLength;
        EnemyDirection = directions.right;
        mode = AIMode.patrol;

    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fEnemyPathEndPoint, transform.position.y);
        startPosition = new Vector2(fEnemyPathStartPoint, transform.position.y);

        TarggetPlayer();
        EnemyMovement();
        SwitchMovementMode();
        FlipEnemy();

    }

    void EnemyMovement()
    {
        switch (mode)
        {
            case AIMode.patrol:
                if (transform.position.x < startPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fEnemySpeed * Time.deltaTime);
                    EnemyDirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fEnemySpeed * Time.deltaTime);
                    EnemyDirection = directions.left;
                    FlipEnemy();
                }




                if (transform.position.x >= startPosition.x && EnemyDirection == directions.right)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fEnemySpeed * Time.deltaTime);
                    if (transform.position.x == endPosition.x)
                    {
                        EnemyDirection = directions.left;
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && EnemyDirection == directions.left)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fEnemySpeed * Time.deltaTime);
                    if (transform.position.x == fEnemyPathStartPoint)
                    {
                        EnemyDirection = directions.right;
                        FlipEnemy();
                    }
                }
                break;

            case AIMode.follow:
                transform.position = Vector2.MoveTowards(transform.position, Target.position, fEnemySpeed * Time.deltaTime);
                break;
        }

    }

    void FlipEnemy()
    {
        // Spriteflip im patrol mode
        switch (EnemyDirection)
        {
            case directions.right:
                GetComponent<SpriteRenderer>().flipX = false;

                switch (GetComponent<EnemyAttack>().EnemyType)
                {
                    case EnemyAttack.Type.commonKnught:
                        CastPointDirection = CastPoint.position + Vector3.right * 5;
                        break;
                    case EnemyAttack.Type.hazardnut:

                        CastPointDirection = CastPoint.position + Vector3.right * 10;
                        break;
                }

                break;

            case directions.left:
                GetComponent<SpriteRenderer>().flipX = true;

                switch (GetComponent<EnemyAttack>().EnemyType)
                {
                    case EnemyAttack.Type.commonKnught:
                        CastPointDirection = CastPoint.position + -Vector3.right * 5;
                        break;

                    case EnemyAttack.Type.hazardnut:
                        CastPointDirection = CastPoint.position + -Vector3.right * 10;
                        break;
                }

                break;
        }


        //Spriteflip im follow mode
        if (mode == AIMode.follow && transform.position.x < Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            EnemyDirection = directions.right;
            CastPointDirection = CastPoint.position + Vector3.right * 5;
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            EnemyDirection = directions.left;
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
            {
                return false;
            }

        }
        return false;
    }

    void SwitchMovementMode()
    {

        if (TarggetPlayer() == true)
        {
            mode = AIMode.follow;
        }
        else
        {
            switch (GetComponent<EnemyAttack>().EnemyType)
            {
                case EnemyAttack.Type.commonKnught:
                    if (Vector3.Distance(transform.position, Target.position) > 5)
                    {
                        mode = AIMode.patrol;
                    }
                    break;
                case EnemyAttack.Type.hazardnut:
                    if (Vector3.Distance(transform.position, Target.position) > 10)
                    {
                        mode = AIMode.patrol;
                    }
                    break;
            }

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



            switch (GetComponent<EnemyAttack>().EnemyType)
            {
                case EnemyAttack.Type.commonKnught:
                    WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Sword);
                    break;

                case EnemyAttack.Type.hazardnut:
                    WeaponDrop.GetComponent<WeaponDropManager>().SetType(Weapons.Weapon.Type.Gloves);
                    break;

            }
        }

    }
}
