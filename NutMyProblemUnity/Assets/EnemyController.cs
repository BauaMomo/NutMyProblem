using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum directions { right, left };
    enum AIMode { follow, patrol };

    public directions Enemydirection;
    AIMode mode;

    Rigidbody2D rb;
    Transform Target;
    [SerializeField] Transform CastPoint;

    Vector2 endPosition;
    Vector2 startPosition;
    Vector3 CastPointDirection;


    float fEnemyPathStartPoint;
    float fEnemyPathEndPoint;
    float fEnemyPathLength;
    float fEnemySpeed;

    // Start is called before the first frame update
    void Start()
    {
        //kann später raus genommen werden und in Unity bestimmt werden -> maybe??!
        // PathStartPoint ist der Spawnpoint des Gegners
        fEnemyPathStartPoint = 4;
        fEnemyPathLength = 6;
        fEnemySpeed = 2;



        Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        fEnemyPathEndPoint = fEnemyPathStartPoint + fEnemyPathLength;
        Enemydirection = directions.right;
        mode = AIMode.patrol;

    }

    // Update is called once per frame
    void Update()
    {
        endPosition = new Vector2(fEnemyPathEndPoint, transform.position.y);
        startPosition = new Vector2(fEnemyPathStartPoint, transform.position.y);

        TarggetPlayer();
        EnemyMovement();
        SwitchMovementmode();
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
                    Enemydirection = directions.right;
                    FlipEnemy();
                }

                if (transform.position.x > endPosition.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fEnemySpeed * Time.deltaTime);
                    Enemydirection = directions.left;
                    FlipEnemy();
                }




                if (transform.position.x >= startPosition.x && Enemydirection == directions.right)
                {
                    transform.position = Vector2.MoveTowards(transform.position, endPosition, fEnemySpeed * Time.deltaTime);
                    if (transform.position.x == endPosition.x)
                    {
                        Enemydirection = directions.left; 
                        FlipEnemy();
                    }
                }

                if (transform.position.x <= endPosition.x && Enemydirection == directions.left)
                {
                    transform.position = Vector2.MoveTowards(transform.position, startPosition, fEnemySpeed * Time.deltaTime);
                    if (transform.position.x == fEnemyPathStartPoint)
                    {
                        Enemydirection = directions.right;
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
        switch (Enemydirection)
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
            Enemydirection = directions.right;
            CastPointDirection = CastPoint.position + Vector3.right * 5;
        }
        if (mode == AIMode.follow && transform.position.x > Target.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            Enemydirection = directions.left;
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
                return true;
            }

            else
            { 
                return false;
            }

        }
        return false;
    }

    void SwitchMovementmode()
    {

        if (TarggetPlayer() == true)
        {
            mode = AIMode.follow;
        }
        else
        {
            if (Vector3.Distance(transform.position, Target.position) > 5)
            {
                mode = AIMode.patrol;
            }
        }
    }
}
