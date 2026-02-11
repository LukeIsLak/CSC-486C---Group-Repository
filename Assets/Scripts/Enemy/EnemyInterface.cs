using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterface : MonoBehaviour
{

    [Header("Enemy Interface Variables")]
    public CharacterData playerData;
    public BaseEnemyData enemyData;
    protected float curHealth;
    protected float moveSpeed;

    public void Awake() {
        initialize();
    }

    protected virtual void initialize() {
        curHealth = enemyData.baseHealth * playerData.maxHealth;
        moveSpeed = enemyData.baseMoveSpeed * playerData.moveSpeed;
    }

    public void KillEnemy() {
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
