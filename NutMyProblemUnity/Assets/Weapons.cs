using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour            //this whole class is kinda hacky and needs a rework
{
    public Weapon Sword;
    public abstract class Weapon
    {
        public playerController playerController;
        public float fColliderSpawnTime;
        public GameObject weaponTrigger;

        public int iDamage { get; protected set; }

        public float iAttackSpeed { get; protected set; }
        public int iRange { get; protected set; }

        public abstract void Attack(playerController.direction direction);


        public class MeleeWeapon : Weapon            //class for swords, spears, boxing gloves...
        {

            public MeleeWeapon(int _iRange, int _iAttack, float _iAttackSpeed)
            {
                iDamage = _iAttack;
                iRange = _iRange;
                iAttackSpeed = _iAttackSpeed;
            }

            public override void Attack(playerController.direction _direction)
            {
                if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / iAttackSpeed))
                {           //spawns the collider to damage enemies
                            //TODO: Make it work for different weapons (currentWeapon variable?)
                    fColliderSpawnTime = Time.fixedUnscaledTime;

                    weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, playerController.transform);     //accessing the playercontroller for the transform probably isnt good
                    weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(iRange, 1);   //problem: scales from the center, overlaps with the player for bigger ranges

                    float fColliderXOffset = 1f;

                    switch (_direction)
                    {
                        case playerController.direction.right:
                            break;
                        case playerController.direction.left:
                            fColliderXOffset *= -1;
                            break;
                    }
                    weaponTrigger.transform.position = playerController.transform.position + new Vector3(fColliderXOffset, -0.2f, 0);
                }
            }
        }

        public class RangedWeapon : Weapon
        {
            public override void Attack(playerController.direction direction)
            {
                throw new System.NotImplementedException();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Sword = new Weapon.MeleeWeapon(1, 20, 4);

        Sword.playerController = GetComponent<playerController>();  //getting this by acessing the sword probably isnt good
    }

    // Update is called once per frame
    void Update()
    {
        if (Sword.fColliderSpawnTime < Time.fixedUnscaledTime - 0.2f) Destroy(Sword.weaponTrigger);
    }

    void SpawnCollider()
    {

    }

}
