using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [field: SerializeField] public int iHealth { get; private set; } = 100;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(iHealth <= 0) Destroy(this.gameObject);
    }

    public void HandleDamage(int _damage, GameObject _other)
    {
        iHealth -= _damage;
        Vector2 directionToOther = (_other.transform.position - this.transform.position).normalized;
        rb.AddForce(new Vector2(-directionToOther.x * 200, 100));
    }
}
