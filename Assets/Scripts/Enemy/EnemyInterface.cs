using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterface : MonoBehaviour
{

    [Header("Enemy Interface Variables")]
    public CharacterData playerData;
    public BaseEnemyData enemyData;
    public EnemyEffects enemyEffects;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float speedModifier = 1;

    public void Awake() {
        initialize();
    }

    public virtual void initialize() {
        /*Initialize enemy data*/
        curHealth = enemyData.baseHealth * playerData.baseHealth;
        moveSpeed = enemyData.baseMoveSpeed * playerData.baseSpeed;
    }

    /*This has the intention of being overwritten in extended classes*/
    public virtual void KillEnemy() {
        Destroy(this.gameObject);
    }

    public void TakeDamage(float amount) {
        curHealth -= amount;
        if (curHealth <= 0) KillEnemy();
    }

    /*This has the intention of being overwritten in extended classes*/
    public virtual void Hit(float damage, float? weight = null, Vector3? colPoint = null) {
        TakeDamage(damage);
    }
}
