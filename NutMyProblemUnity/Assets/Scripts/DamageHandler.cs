using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] float knockbackFactor;

    [field: SerializeField] public int iHealth { get; private set; } = 100;
    public Rigidbody2D rb;
    public bool isInvincible { get; protected set; } = false;

    HazardnutAnimationController hAnimationController;
    CommonKnughtAnimationController cAnimationController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (this.tag == "Hazardnut") hAnimationController = GetComponent<HazardnutAnimationController>();
        if (this.tag == "CommonKnught") cAnimationController = GetComponent<CommonKnughtAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (iHealth <= 0)
        {
            switch (this.tag)
            {
                case "Player":
                    break;

                case "CommonKnught":
                    GetComponent<CommonKnughtController>().CommonKnughtDeath();
                    break;

                case "Hazardnut":
                    GetComponent<HazardnutController>().HazardnutDeath();
                    FindObjectOfType<AudioManager>().Stop("HazardnutStopAttack");
                    break;
            }
        }
    }

    public void StartInvincibility(float _time)
    {
        isInvincible = true;
        Invoke(nameof(EndInvincibility), _time);
    }

    void EndInvincibility()
    {
        isInvincible = false;
    }

    public void HandleHealing(int _ammount)
    {
        iHealth = Mathf.Clamp(iHealth + _ammount, 0, 100);
    }

    public void HandleDamage(int _amount, GameObject _other)
    {
        if (!isInvincible) iHealth-= _amount;
        if(iHealth<=0)
        {
            return;
        }
        if (this.tag == "Player")
        {
            FindObjectOfType<AudioManager>().Play("PlayerDamage");
            switch (_other.tag)
            {
                case "Spikes":
                    StartInvincibility(1f);
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, 1000));
                    break;
                case "CommonKnught":
                    StartInvincibility(1f);
                    GetComponent<playerController>().DisableMovementFor(0.3f);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    rb.AddForce(new Vector2(-Mathf.Sign(_other.transform.position.x - transform.position.x), 0.5f) * 700);
                    break;
                case "Hazardnut":
                    StartInvincibility(1f);
                    GetComponent<playerController>().DisableMovementFor(0.6f);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    rb.AddForce(new Vector2(-Mathf.Sign(_other.transform.position.x - transform.position.x), 0.5f) * 1700 * knockbackFactor);
                    break;
            }

        }
        if (this.tag == "CommonKnught")
        {
            StartInvincibility(0.3f);
            cAnimationController.OnDamaged.Invoke();
            GetComponent<CommonKnughtController>().DisableMovementFor(0.5f);
            Vector2 directionToOther = (_other.transform.position - this.transform.position).normalized;
            Vector2 playerForceVector = _other.GetComponent<Weapons>().currentWeapon.KnockbackVector;
            rb.AddForce(new Vector2((-directionToOther.x * playerForceVector.x), playerForceVector.y) * 20);

            if (_other.GetComponent<Weapons>().currentWeapon.WeaponType == Weapons.Weapon.Type.Sword)
            {
                FindObjectOfType<AudioManager>().Play("EnemyGetDamage_Sword");
            }

            if (_other.GetComponent<Weapons>().currentWeapon.WeaponType == Weapons.Weapon.Type.Gloves)
            {
                FindObjectOfType<AudioManager>().Play("EnemyGetDamage_Gloves");
            }
            if (_other.GetComponent<Weapons>().currentWeapon.WeaponType == Weapons.Weapon.Type.Fists)
            {
                FindObjectOfType<AudioManager>().Play("EnemyGetDamage_Fists");
            }
        }

        if (this.tag == "Hazardnut")
        {
            StartInvincibility(0.3f);
            hAnimationController.OnDamaged.Invoke();
            GetComponent<HazardnutController>().DisableMovementFor(0.5f);
            Vector2 directionToOther = (_other.transform.position - this.transform.position).normalized;
            Vector2 playerForceVector = _other.GetComponent<Weapons>().currentWeapon.KnockbackVector;
            rb.AddForce(Vector2.Scale(-directionToOther * 5000, new Vector2(1, 0.2f)));

            if (_other.GetComponent<Weapons>().currentWeapon.WeaponType == Weapons.Weapon.Type.Sword)
            {
                FindObjectOfType<AudioManager>().Play("EnemyGetDamage_Sword");
            }

            if (_other.GetComponent<Weapons>().currentWeapon.WeaponType == Weapons.Weapon.Type.Gloves)
            {
                FindObjectOfType<AudioManager>().Play("EnemyGetDamage_Gloves");
            }

        }
    }
    public void HandleDamage(int _amount)
    {
        if (isInvincible) return;
        iHealth -= _amount;

        if (this.tag == "Player")
        {
            StartInvincibility(0.5f);
            FindObjectOfType<AudioManager>().Play("PlayerDamage");

        }
    }
    public void ResetHealth()
        { iHealth = 100; }
}
