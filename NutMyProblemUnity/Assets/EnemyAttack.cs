using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class EnemyAttack : MonoBehaviour
{

    public enum Type { commonKnught }

    public Type EnemyType;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject AttackRangeRight;
    [SerializeField] GameObject AttackRangeLeft;
    [SerializeField] Transform TPlayer;

    int iSwordDamage;
    int iDamageTimer;
    [SerializeField] int iDamageTime;

    [SerializeField] bool bEnemyAttackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        iSwordDamage = 20;
        iDamageTimer = 30;
        iDamageTime = 0;
        bEnemyAttackCooldown = false;
        AttackRangeRight.SetActive(false);
        AttackRangeLeft.SetActive(false);

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<DamageHandler>().iHealth <= 0)
        { EditorSceneManager.LoadScene("SampleScene"); }

        if (bEnemyAttackCooldown == true)
        {
            iDamageTime++;
            if (iDamageTime >= iDamageTimer)
            {
                bEnemyAttackCooldown = false;
            }
        }
        if (iDamageTime >= iDamageTimer)
               iDamageTime = 0;

        if (Vector3.Distance(transform.position, TPlayer.position) < 2 && GetComponent<EnemyController>().Enemydirection == EnemyController.directions.right)
            AttackRangeRight.SetActive(true);


        if (Vector3.Distance(transform.position, TPlayer.position) < 2 && GetComponent<EnemyController>().Enemydirection == EnemyController.directions.left)
            AttackRangeLeft.SetActive(true);

        if (Vector3.Distance(transform.position, TPlayer.position) > 1)
        {
            AttackRangeRight.SetActive(false);
            AttackRangeLeft.SetActive(false);
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (EnemyType)
            {
                case Type.commonKnught:
                    if (bEnemyAttackCooldown == false)
                        SwordAttack(iSwordDamage);
                    break;
            }
        }



    }
    void SwordAttack(int AttackDamage)
    {
        bEnemyAttackCooldown = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<DamageHandler>().iHealth -= AttackDamage;
    }
}
