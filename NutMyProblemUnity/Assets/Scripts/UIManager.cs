using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    GameObject player;
    DamageHandler damageHandler;
    Weapons weapons;

    TMPro.TextMeshProUGUI hpText;
    TMPro.TextMeshProUGUI weaponTypeText;

    Sprite FullHeart;
    Sprite EmptyHeart;
    Sprite WeaponSwordActive;
    Sprite WeaponSwordInactive;
    Sprite WeaponGlovesActive;
    Sprite WeaponGlovesInactive;
    Sprite EmptySlot;

    GameObject HeartEmpty;
    List<GameObject> HeartsGOs = new List<GameObject>();

    GameObject WeaponEmpty;
    List<GameObject> WeaponGOs = new List<GameObject>();

    GameObject WeaponSwitchTutorial;

    bool WeaponSwitchTutorialGate = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<GameManager>().GetComponent<GameManager>().player;
        damageHandler = player.GetComponent<DamageHandler>();
        weapons = player.GetComponent<Weapons>();

        FullHeart = Resources.Load<Sprite>("User_Interface_Sprites/Heart");
        EmptyHeart = Resources.Load<Sprite>("User_Interface_Sprites/Heart_Lost");

        WeaponSwordActive = Resources.Load<Sprite>("User_Interface_Sprites/Symbol_Sword_On");
        WeaponSwordInactive = Resources.Load<Sprite>("User_Interface_Sprites/Symbol_Sword_Off");
        WeaponGlovesActive = Resources.Load<Sprite>("User_interface_Sprites/Symbol_Gloves_On");
        WeaponGlovesInactive = Resources.Load<Sprite>("User_Interface_Sprites/Symbol_Gloves_Off");
        EmptySlot = Resources.Load<Sprite>("User_Interface_Sprites/EmptySlot");


        HeartEmpty = transform.Find("Canvas/Hearts").gameObject;
        for (int i = 0; i < HeartEmpty.transform.childCount; i++)
        {
            HeartsGOs.Add(HeartEmpty.transform.Find("Heart " + i).gameObject);
        }

        WeaponEmpty = transform.Find("Canvas/Weapons").gameObject;
        for (int i = 0; i < WeaponEmpty.transform.childCount; i++)
        {
            WeaponGOs.Add(WeaponEmpty.transform.Find("Weapon " + i).gameObject);
        }

        WeaponSwitchTutorial = transform.Find("Canvas/WeaponSwitchTutorial").gameObject;
        WeaponSwitchTutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        SetHealthUI();
        SetWeaponUI();

        if(!WeaponSwitchTutorialGate && weapons.availableWeapons.Count > 1)
        {
            WeaponSwitchTutorialGate = true;
            WeaponSwitchTutorial.SetActive(weapons.availableWeapons.Count > 1);
        }
    }
    void SetWeaponUI()
    {
        WeaponGOs[0].GetComponent<Image>().sprite = GetSpriteFromWeapon(weapons.currentWeapon, true);

        List<Weapons.Weapon> WeaponsWithoutCurrent = new List<Weapons.Weapon>();
        foreach(Weapons.Weapon weapon in weapons.availableWeapons)
        {
            WeaponsWithoutCurrent.Add(weapon);
        }
        WeaponsWithoutCurrent.Remove(weapons.currentWeapon);

        if (WeaponsWithoutCurrent.Count > 0)
            WeaponGOs[1].GetComponent<Image>().sprite = GetSpriteFromWeapon(WeaponsWithoutCurrent[0], false);
    }
    Sprite GetSpriteFromWeapon(Weapons.Weapon _weapon, bool _isActive)
    {
        if (_isActive)
        {
            if (_weapon.WeaponType == Weapons.Weapon.Type.Sword) return WeaponSwordActive;
            if (_weapon.WeaponType == Weapons.Weapon.Type.Gloves) return WeaponGlovesActive;
        }
        else
        {
            if (_weapon.WeaponType == Weapons.Weapon.Type.Sword) return WeaponSwordInactive;
            if (_weapon.WeaponType == Weapons.Weapon.Type.Gloves) return WeaponGlovesInactive;
        }
        return EmptySlot;
    }
    void SetHealthUI()
    {
        int i = damageHandler.iHealth;

        foreach (GameObject heart in HeartsGOs)
        {
            if (i > 0) heart.GetComponent<Image>().sprite = FullHeart;
            else heart.GetComponent<Image>().sprite = EmptyHeart;

            i -= 20;
        }
    }
}
