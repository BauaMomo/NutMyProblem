using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    GameObject player;
    DamageHandler damageHandler;
    Weapons weapons;

    TMPro.TextMeshProUGUI hpText;
    TMPro.TextMeshProUGUI weaponTypeText;

    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<GameManager>().GetComponent<GameManager>().player;
        damageHandler = player.GetComponent<DamageHandler>();
        weapons = player.GetComponent<Weapons>();
        hpText = transform.Find("Canvas/HPText").GetComponent<TMPro.TextMeshProUGUI>();
        weaponTypeText = transform.Find("Canvas/WeaponTypeText").GetComponent<TMPro.TextMeshProUGUI>();

    }

    // Update is called once per frame
    void Update()
    {
        hpText.SetText(damageHandler.iHealth.ToString());
        weaponTypeText.SetText(weapons.currentWeapon.WeaponType.ToString());
    }
}
