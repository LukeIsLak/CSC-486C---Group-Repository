using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterface : MonoBehaviour
{
    public CharacterData playerData;
    public BaseEnemyData enemyData;


    protected float curHealth;
    // protected double moveSpeed;

    public void Awake() {

    }

    // public int getHealth() {
    //     return health;
    // }

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
