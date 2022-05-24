using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour            //this whole class is kinda hacky and needs a rework
{
    Weapon sword;
    Weapon bow;
    public Weapon currentWeapon;
    List<Weapon> availableWeapons = new List<Weapon>();

    public abstract class Weapon
    {
        //constructor that every weapon type inherits
        public Weapon(float _iRange, int _iAttack, float _iAttackSpeed)
        {
            iDamage = _iAttack;
            iRange = _iRange;
            iAttackSpeed = _iAttackSpeed;
        }

        public playerController playerController;
        public GameObject player;

        public float fColliderSpawnTime;
        public GameObject weaponTrigger;

        public int iDamage { get; protected set; }
        public float iAttackSpeed { get; protected set; }
        public float iRange { get; protected set; }

        public abstract void Attack(playerController.direction direction);

        public class Sword : Weapon
        {

            public Sword(float _iRange, int _iAttack, float _iAttackSpeed) : base(_iRange, _iAttack, _iAttackSpeed)   {}

            public override void Attack(playerController.direction _direction)
            {
                if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / iAttackSpeed))
                {           
                    //spawns the collider to damage enemies
                    fColliderSpawnTime = Time.fixedUnscaledTime;

                    weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, player.transform);
                    weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(iRange, 1);

                    float fColliderXOffset = 0.5f + iRange / 2;

                    switch (_direction)     //decide on which side of the player the collider will be
                    {
                        case playerController.direction.right:
                            break;
                        case playerController.direction.left:
                            fColliderXOffset *= -1;
                            break;
                    }
                    weaponTrigger.transform.position = playerController.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);

                    Destroy(weaponTrigger, 0.2f);       //Destroy the collider after x seconds
                }
            }
        }

        public class Bow : Weapon
        {
            public Bow(float _iRange, int _iAttack, float _iAttackSpeed) : base(_iRange, _iAttack, _iAttackSpeed) { }
            public override void Attack(playerController.direction direction)
            {
                throw new System.NotImplementedException();
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        sword = new Weapon.Sword(1.5f, 20, 4);
        bow = new Weapon.Bow(1, 20, 2);

        availableWeapons.Add(sword);
        availableWeapons.Add(bow);

        currentWeapon = availableWeapons[0];

        currentWeapon.playerController = GetComponent<playerController>();  
        currentWeapon.player = currentWeapon.playerController.gameObject;
    }

    public void SwitchWeapon()
    {
        //gets the index of currentWeapon, switches to the next Weapon in the List or to the first one if the index is the last in the list

        int currentWeaponIndex = availableWeapons.FindIndex(weapon => weapon == currentWeapon);
        if(currentWeaponIndex < availableWeapons.Count-1) currentWeapon = availableWeapons[currentWeaponIndex+1];
        else currentWeapon = availableWeapons[0];

        //Debug.Log(currentWeapon);
    }
}
