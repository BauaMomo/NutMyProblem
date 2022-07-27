using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Weapons : MonoBehaviour
{
    public Weapon sword { get; protected set; }
    public Weapon bow { get; protected set; }
    public Weapon gloves { get; protected set; }
    public Weapon fists { get; protected set; }
    public Weapon currentWeapon { get; protected set; }
    List<Weapon> allWeapons;
    List<Weapon> availableWeapons;

    public UnityEvent onAttack = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        sword = gameObject.AddComponent<Sword>();
        gloves = gameObject.AddComponent<Gloves>();
        fists = gameObject.AddComponent<Fists>();

        allWeapons = new List<Weapon> { sword, gloves, fists };     //all weapons the player could pick up
        availableWeapons = new List<Weapon> { fists, sword };               //all weapons the player currently has

        currentWeapon = availableWeapons[0];                        //the weapon the player has equipped
    }

    public void SwitchWeapon()
    {
        //gets the index of currentWeapon, switches to the next Weapon in the List or to the first one if the index is the last in the list

        int currentWeaponIndex = availableWeapons.FindIndex(weapon => weapon == currentWeapon);
        if (currentWeaponIndex < availableWeapons.Count - 1) currentWeapon = availableWeapons[currentWeaponIndex + 1];
        else currentWeapon = availableWeapons[0];
    }

    void SwitchToWeapon(Weapon _switchToThis)
    {
        if (_switchToThis == null)
        {
            Debug.LogWarning("The weapon you're trying to switch to is null!");
            return;
        }
        if (availableWeapons.Find(weapon => weapon == _switchToThis) == null)
        {
            Debug.LogWarning("The weapon you're trying to switch to is not in availableWeapons!");
            return;      //returns if the player doesn't have the weapon _findThisWeapon
        }

        currentWeapon = _switchToThis;
    }

    public void AddWeaponFromDrop(GameObject _drop)
    {
        if (_drop == null) return;

        Weapon weaponToAdd = allWeapons.Find(weapon => weapon.WeaponType == _drop.GetComponent<WeaponDropManager>().WeaponType);    //finds the weapon that should be added to the player's inventory based on the weaponType of the drop

        foreach (Weapon weapon in availableWeapons)      //returns if the player already has that weapon
        {
            if (weapon == weaponToAdd)
            {
                SwitchToWeapon(weaponToAdd);
                return;
            }
        }

        availableWeapons.Add(weaponToAdd);
        if (availableWeapons.Find(weapon => weapon == fists) != null) availableWeapons.Remove(fists);   //if the player has the fists, remove them.

        SwitchToWeapon(weaponToAdd);
    }

    public class Weapon : MonoBehaviour
    {
        public enum Type { Sword, Bow, Gloves, Fists };
        public Type WeaponType { get; protected set; }

        protected playerController playerController;
        protected Rigidbody2D rb;
        protected GameObject player;

        protected float fColliderSpawnTime;
        protected GameObject weaponTrigger;

        public int iDamage { get; protected set; }
        public float fAttackSpeed { get; protected set; }       //Attack speed in hits per second
        public float fRange { get; protected set; }
        public float attackMoveStrength { get; protected set; }
        public Vector2 KnockbackVector { get; protected set; }      //The direction and strength the enemy gets pushed in when hit

        public virtual IEnumerator Attack(playerController.direction direction)
        {
            Debug.LogWarning("No Attack function found");
            yield return new WaitForSeconds(0f);
        }

        public void SetUniversalVars(Weapon weapon)     //Sets variables that are the same in all weapons
        {
            weapon.player = this.gameObject;
            weapon.playerController = GetComponent<playerController>();
            weapon.rb = GetComponent<Rigidbody2D>();
        }
    }

    public class Sword : Weapon
    {
        private void Start()
        {
            WeaponType = Type.Sword;
            iDamage = 20;
            fAttackSpeed = 2;
            fRange = 3f;
            attackMoveStrength = 4;
            KnockbackVector = new Vector2(50, 20);
            SetUniversalVars(this);
        }

        public override IEnumerator Attack(playerController.direction _direction)
        {
            if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fAttackSpeed))
            {
                GetComponent<Weapons>().onAttack.Invoke();
                //spawns the collider to damage enemies
                fColliderSpawnTime = Time.fixedUnscaledTime;

                weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, player.transform);
                weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(fRange, 3);

                Vector2 ColliderOffset = new Vector2(-0.7f + fRange / 2, 0);
                Vector2 attackMoveForce = new Vector2(100*attackMoveStrength, 0 * attackMoveStrength);

                switch (_direction)     //decide on which side of the player the collider will be
                {
                    case playerController.direction.right:
                        attackMoveForce.x *= 1;
                        break;
                    case playerController.direction.left:
                        attackMoveForce.x *= -1;
                        ColliderOffset.x *= -1;
                        break;
                }
                rb.AddForce(attackMoveForce);

                weaponTrigger.transform.position = playerController.transform.position + (Vector3)ColliderOffset;

                Destroy(weaponTrigger, 0.3f);       //Destroy the collider after x seconds
            }
            yield return new WaitForSeconds(0f);
        }
    }

    public class Bow : Weapon
    {
        private void Start()
        {
            WeaponType = Type.Bow;
            iDamage = 20;
            fAttackSpeed = 4;
            fRange = 1.5f;
            SetUniversalVars(this);
        }
    }

    public class Gloves : Weapon
    {
        private void Start()
        {
            WeaponType = Type.Gloves;
            iDamage = 40;
            fAttackSpeed = 0.8f;
            fRange = 3f;
            attackMoveStrength = 1000;
            KnockbackVector = new Vector2(180, 50);
            SetUniversalVars(this);
        }

        public override IEnumerator Attack(playerController.direction _direction)
        {
            if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fAttackSpeed))
            {
                GetComponent<Weapons>().onAttack.Invoke();
                //spawns the collider to damage enemies
                fColliderSpawnTime = Time.fixedUnscaledTime;

                playerController.DisableMovementFor(0.8f);
                yield return new WaitForSeconds(0.3f);

                weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, player.transform);
                weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(fRange, 1.5f);

                Vector2 ColliderOffset = new Vector2(0.5f + fRange / 2, 0.2f);

                Vector2 attackMoveForce = new Vector2(100, 0).normalized * attackMoveStrength;

                switch (_direction)     //decide on which side of the player the collider will be
                {
                    case playerController.direction.right:
                        attackMoveForce.x *= 1;
                        break;
                    case playerController.direction.left:
                        attackMoveForce.x *= -1;
                        ColliderOffset.x *= -1;
                        break;
                }
                rb.AddForce(attackMoveForce);

                weaponTrigger.transform.position = playerController.transform.position + (Vector3)ColliderOffset;

                Destroy(weaponTrigger, 0.5f);       //Destroy the collider after x seconds
            }
        }
    }

    public class Fists : Weapon
    {
        private void Start()
        {
            WeaponType = Type.Fists;
            iDamage = 10;
            fAttackSpeed = 2;
            fRange = 1.5f;
            attackMoveStrength = 300;
            KnockbackVector = new Vector2(120, 80);
            SetUniversalVars(this);
        }
        public override IEnumerator Attack(playerController.direction _direction)
        {
            //THIS IS A COPY OF THE SWORD ATTACK AND SHOULD BE CHANGED!

            if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fAttackSpeed))
            {
                GetComponent<Weapons>().onAttack.Invoke();
                //spawns the collider to damage enemies
                fColliderSpawnTime = Time.fixedUnscaledTime;

                weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, player.transform);
                weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(fRange, 2);

                Vector2 ColliderOffset = new Vector2(0.5f + fRange / 2, 0);

                Vector2 attackMoveForce = new Vector2(100, 100).normalized * attackMoveStrength;

                switch (_direction)     //decide on which side of the player the collider will be
                {
                    case playerController.direction.right:
                        attackMoveForce.x *= 1;
                        break;
                    case playerController.direction.left:
                        attackMoveForce.x *= -1;
                        ColliderOffset.x *= -1;
                        break;
                }
                rb.AddForce(attackMoveForce);

                weaponTrigger.transform.position = playerController.transform.position + (Vector3)ColliderOffset;

                Destroy(weaponTrigger, 0.1f);       //Destroy the collider after x seconds
            }
            yield return new WaitForSeconds(0f);
        }
    }
}
