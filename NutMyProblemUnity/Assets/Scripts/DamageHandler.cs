using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [field: SerializeField] public int iHealth { get; private set; } = 100;
    Rigidbody2D rb;
    bool isInvincible = false;

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

    void StartInvincibility()
    {
        isInvincible = true;
        Invoke(nameof(EndInvincibility), 0.5f);
    }

    void EndInvincibility()
    {
        isInvincible = false;
    }

    public void HandleDamage(int _damage, GameObject _other)
    {
        if(isInvincible) return;
        iHealth -= _damage;
        if(this.tag == "Player")
        {
            StartInvincibility();

            switch (_other.tag)
            {
                case "Spikes":
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, 1000));
                    break;
            }
        }
        if (this.tag == "CommonKnught" || this.tag == "Hazardnut")
        {
            if(this.tag == "Hazardnut") anim.OnDamaged.Invoke();

            Vector2 directionToOther = (_other.transform.position - this.transform.position).normalized;
            Vector2 playerForceVector = _other.GetComponent<Weapons>().currentWeapon.KnockbackVector;
            rb.AddForce(new Vector2((-directionToOther.x * playerForceVector.x), playerForceVector.y) * 4);
        }
    }
}
