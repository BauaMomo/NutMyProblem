using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDropManager : MonoBehaviour
{
    public Weapons.Weapon.Type WeaponType;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        
    }

    public void SetType(Weapons.Weapon.Type _weaponType)
    {
        WeaponType = _weaponType;
        switch (WeaponType)
        {
            case Weapons.Weapon.Type.Sword:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Schwert");
                //spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/DropSpriteSword");
                break;
            case Weapons.Weapon.Type.Gloves:
                //spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/DropSpriteGloves");
                break;
            case Weapons.Weapon.Type.Fists:
                break;
            default:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Schwert");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
