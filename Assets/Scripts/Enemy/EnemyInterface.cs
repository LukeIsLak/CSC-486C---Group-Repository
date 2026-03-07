using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterface : MonoBehaviour
{

    [Header("Enemy Interface - Base Variables")]
    public CharacterData playerData;
    public BaseEnemyData enemyData;
    public EnemyEffects enemyEffects;
    public TrapRoomSpawner trs;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float speedModifier = 1;

    [Header("Enemy Interface - Effect Variables")]
    public bool hasDamageOverTime   = false;
    public bool hasFreeze           = false;
    public bool hasKnockback        = false;

    public int numDamageOverTime    = 0;
    public int numFreeze            = 0;

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
        if (trs != null) trs.RemoveEnemy();
        Destroy(this.gameObject);
    }

    public void TakeDamage(float amount) {
        curHealth -= (hasFreeze)? amount * enemyData.freezeMult : amount;
        if (curHealth <= 0) KillEnemy();
    }

    /*This has the intention of being overwritten in extended classes*/
    public virtual void Hit(float damage, StatusEffectType status = StatusEffectType.None, StatusEffects? statusEffectData = null) {

        switch (status) {
            case StatusEffectType.DamageOverTime:
                break;
            case StatusEffectType.Freeze:
                break;
            case StatusEffectType.Knockback:
                break;
            default:
                break;
        }
        TakeDamage(damage);
    }
}
