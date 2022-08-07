using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public Weapons.Weapon.Type WeaponType;
    SpriteRenderer spriteRenderer;

    public ParticleSystem EnemyDeathParticle;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        EnemyDeathParticle.Play();
    }

    public void SetType(Weapons.Weapon.Type _weaponType)
    {
        WeaponType = _weaponType;
        switch (WeaponType)
        {
            case Weapons.Weapon.Type.Sword:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/DropSprite_Sword");
                break;
            case Weapons.Weapon.Type.Gloves:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/DropSprite_Gloves");
                break;
            case Weapons.Weapon.Type.Fists:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
