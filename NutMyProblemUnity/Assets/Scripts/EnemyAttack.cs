using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public enum Type { commonKnught, hazardnut }

    public Type EnemyType;

    [SerializeField] GameObject Player;
    [SerializeField] GameObject Enemy;
    GameObject weaponTrigger;

    public Transform TPlayer;

    public int iSwordDamage;
    public int iGlovesDamage;

    float fColliderSpawnTime;
    public float iAttackSpeed { get; protected set; }
    public float fGlovesAttackSpeed { get; protected set; }
    public float iRange { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        iSwordDamage = 20;
        iGlovesDamage = 30;
        iAttackSpeed = 1;
        fGlovesAttackSpeed = 0.5f;
        iRange = 1.5f;

        TPlayer = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        switch (EnemyType)
        {
            case Type.commonKnught:
                if (Vector3.Distance(transform.position, TPlayer.position) < 1)
                    SwordAttack(GetComponent<EnemyController>().EnemyDirection);
                break;

            case Type.hazardnut:
                if (Vector3.Distance(transform.position, TPlayer.position) < 8)
                    GlovesAttack(GetComponent<EnemyController>().EnemyDirection);
                break;
        }

        
    }

    public void SwordAttack(EnemyController.directions _directions)
    {

        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / iAttackSpeed))
        {
            fColliderSpawnTime = Time.fixedUnscaledTime;

            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, Enemy.transform);
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(iRange, 1);

            float fColliderXOffset = 0.5f + iRange / 2;

            switch (_directions)
            {
                case EnemyController.directions.right:
                    break;
                case EnemyController.directions.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponTrigger.transform.position = Enemy.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);

            Destroy(weaponTrigger, 0.1f);
        }
    }
    public void GlovesAttack(EnemyController.directions _directions)
    {

        if (fColliderSpawnTime < Time.fixedUnscaledTime - (1 / fGlovesAttackSpeed))
        {
            fColliderSpawnTime = Time.fixedUnscaledTime;

            weaponTrigger = Instantiate(Resources.Load("prefabs/WeaponTrigger") as GameObject, Enemy.transform);
            weaponTrigger.GetComponent<BoxCollider2D>().size = new Vector2(iRange, 1);

            float fColliderXOffset = 0.5f + iRange / 2;

            switch (_directions)
            {
                case EnemyController.directions.right:
                    break;
                case EnemyController.directions.left:
                    fColliderXOffset *= -1;
                    break;
            }
            weaponTrigger.transform.position = Enemy.transform.position + new Vector3(fColliderXOffset, -0.1f, 0);

            Destroy(weaponTrigger, 0.1f);
        }

    }
}
