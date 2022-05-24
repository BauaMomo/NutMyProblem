using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 newPosition;
    [SerializeField] float fEnemySpeed;

    // Start is called before the first frame update
    void Start()
    {
        startPosition.x = transform.position.x;
        startPosition.y = -0.38f;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        newPosition.x = startPosition.x;
        newPosition.y = startPosition.y;
        newPosition.x = newPosition.x + Mathf.PingPong(Time.time * fEnemySpeed, 6);
        if(newPosition.x >= 9.9f)
            GetComponent<SpriteRenderer>().flipX = true;        
        if(newPosition.x <= 4.1f)
            GetComponent<SpriteRenderer>().flipX = false;

        transform.position = newPosition;


    }
}
