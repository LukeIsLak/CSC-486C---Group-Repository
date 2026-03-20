using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInterface : MonoBehaviour
{

    [Header("Enemy Interface - Base Variables")]
    public CharacterData playerData;
    public BaseEnemyData enemyData;
    public EnemyEffects enemyEffects;
    public RoomSpawn rs;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float speedModifier = 1;

    [Header("Enemy Interface - Effect Variables")]
    public bool hasDamageOverTime   = false;
    public bool hasFreeze           = false;
    public bool hasKnockback        = false;

    public int numDamageOverTime    = 0;
    public int numFreeze            = 0;

    [Header("Enemy Interface - Damage Over Time")]
    public List<string> currentDoTNames  = new List<string>();
    public List<int> currentDoTTicks     = new List<int>();

    [Header("Enemy Interface - Enemy Effects")]
    public bool hasBlood = true;
    public float bloodDuration = 1.0f;
    public GameObject bloodEffect;

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
        if (rs != null) rs.RemoveEnemy();
        Destroy(this.gameObject);
    }

    public void TakeDamage(float amount) {
        if(curHealth <= 0) return;
        curHealth -= (hasFreeze)? amount * enemyData.freezeMult : amount;
        if (curHealth <= 0) KillEnemy();
    }

    /*This has the intention of being overwritten in extended classes*/
    public virtual void Hit(float damage, StatusEffectType status = StatusEffectType.None, StatusEffects? statusEffectData = null, Vector3? knockbackOrigin = null) {
        if (curHealth <= 0) return;
        if (curHealth > 0) TakeDamage(damage);
        if (status == StatusEffectType.None && hasBlood) StartCoroutine(AddBlood());

        switch (status) {
            case StatusEffectType.DamageOverTime:
                DamageOverTime dot = statusEffectData as DamageOverTime;
                HandleDamageOverTime(dot);
                break;
            case StatusEffectType.Freeze:
                Freeze freeze = statusEffectData as Freeze;
                HandleFreeze(freeze);
                break;
            case StatusEffectType.Knockback:
                Knockback knockback = statusEffectData as Knockback;
                if (knockbackOrigin != null) HandleKnockback(knockback, knockbackOrigin.Value);
                break;
            default:
                break;
        }
    }

    /****************************************************/
    /*           Beginning Of Status Methods            */
    /****************************************************/

    public void HandleDamageOverTime(DamageOverTime data) {
        hasDamageOverTime = true;
        int index = currentDoTNames.IndexOf(data.name);
        if (index != -1 && !data.isStackable) {
            currentDoTTicks[index] += data.totalTicks;
        }
        else {
            numDamageOverTime += 1;
            currentDoTNames.Add(data.name);
            currentDoTTicks.Add(data.totalTicks);
            StartCoroutine(StartDamageOverTime(data));
        }
    }

    private IEnumerator StartDamageOverTime(DamageOverTime data) {
        int index = currentDoTNames.IndexOf(data.name);
        for (int i = 0; i < currentDoTTicks[index]; i++) {
            TakeDamage(data.tickDamage);
            yield return new WaitForSeconds(1f / data.ticksPerSecond);
            index = currentDoTNames.IndexOf(data.name);
        }

        currentDoTNames.RemoveAt(index);
        currentDoTTicks.RemoveAt(index);
        if (--numDamageOverTime <= 0) hasDamageOverTime = false;
    }

    public void HandleFreeze(Freeze data) {
        StartCoroutine(StartFreeze(data));
    }

    private IEnumerator StartFreeze(Freeze data) {
        hasFreeze = true;
        numFreeze += 1;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer sr in sprites) {
            originalColors.Add(sr.color);
            sr.color = Color.blue;
        }

        yield return new WaitForSeconds(data.freezeDuration);

        if (--numFreeze <= 0) {
            for (int i = 0; i < sprites.Length; i++) {
                if (sprites[i] != null)
                    sprites[i].color = originalColors[i];
            }
            hasFreeze = false;
        }
    }

    public void HandleKnockback(Knockback data, Vector3 knockbackOrigin) {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (knockbackOrigin == null) return;
        if (rb != null) {
            Vector3 direction = (transform.position - knockbackOrigin).normalized;
            // direction.y = 0f; leave in if we want 
            rb.AddForce(direction * data.knockbackForce, ForceMode.Force);
            // hasKnockback = true;
        }
    }

    /****************************************************/
    /*              End Of Status Methods               */
    /****************************************************/



    /****************************************************/
    /*               Beginning Of Effects               */
    /****************************************************/

    private IEnumerator AddBlood() {
        GameObject blood = GameObject.Instantiate(bloodEffect, transform.position, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(bloodDuration);
        Destroy(blood);
    }

    /****************************************************/
    /*                 End Of Effects                   */
    /****************************************************/
}
