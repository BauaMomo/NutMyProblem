using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    TMPro.TextMeshProUGUI hpText;
    // Start is called before the first frame update
    void Start()
    {
        hpText = transform.Find("Canvas/EnemyHPText").GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        hpText.SetText(GetComponent<DamageHandler>().iHealth.ToString());
    }
}
