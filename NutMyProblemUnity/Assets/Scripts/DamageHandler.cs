using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [field: SerializeField] public int iHealth { get; private set; } = 100;
    Rigidbody2D rb;

    HazardnutAnimationController anim;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(this.tag == "Hazardnut") anim = GetComponent<HazardnutAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iHealth <= 0)
        {
            switch (this.tag)
            {
                case "Player":
                    Destroy(this.gameObject);
                    break;

                case "CommonKnught":
                        GetComponent<CommonKnughtController>().CommonKnughtDeath();
                    break;

                case "Hazardnut":
                        GetComponent<HazardnutController>().HazardnutDeath();
                    break;
            }
        }
    }

    public void HandleDamage(int _damage, GameObject _other)
    {
        iHealth -= _damage;
        if (this.tag == "CommonKnught" || this.tag == "Hazardnut")
        {
            if(this.tag == "Hazardnut") anim.OnDamaged.Invoke();

            Vector2 directionToOther = (_other.transform.position - this.transform.position).normalized;
            Vector2 playerForceVector = _other.GetComponent<Weapons>().currentWeapon.KnockbackVector;
            rb.AddForce(new Vector2((-directionToOther.x * playerForceVector.x), playerForceVector.y) * 4);
        }
    }
}
