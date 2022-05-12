using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    float fColliderSpawnTime;
    GameObject weaponCollider;
    playerController playerController;
    public HandWeapon Sword;

    public class HandWeapon
    {
        public int iDamage { get; }
        public int iRange { get; }
        public float iAttackSpeed { get; }

        public HandWeapon(int _iRange, int _iAttack, float _iAttackSpeed)
        {
            iDamage = _iAttack;
            iRange = _iRange;
            iAttackSpeed = _iAttackSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<playerController>();
        Sword = new HandWeapon(1, 20, 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (fColliderSpawnTime < Time.fixedUnscaledTime - 0.1f) Destroy(weaponCollider);
    }
    public void Attack(playerController.direction _direction)
    {
        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / Sword.iAttackSpeed))
        {
            fColliderSpawnTime = Time.fixedUnscaledTime;

            weaponCollider = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, this.transform);
            weaponCollider.GetComponent<BoxCollider2D>().size = new Vector2(Sword.iRange, 1);

            float fColliderXOffset = 1f;

            switch (_direction)
            {
                case playerController.direction.right:
                    break;
                case playerController.direction.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponCollider.transform.position = this.transform.position + new Vector3(fColliderXOffset, -0.2f, 0);
        }
    }

    
}
