using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInterface : MonoBehaviour
{

    private uint fillerFlag = 0;

    [Header("Enemy Interface - Base Variables")]
    public CharacterData playerData;
    public BaseEnemyData enemyData;
    public EnemyEffects enemyEffects;
    public RoomSpawn rs;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float speedModifier = 1;

    public bool isDead = false;

    [Header("Enemy Interface - Effect Variables")]
    public bool hasDamageOverTime   = false;
    public bool hasFreeze           = false;
    public bool hasKnockback        = false;
    public bool isStopped           = false;

    public bool isDying             = false;

    public int numDamageOverTime    = 0;
    public int numFreeze            = 0;

    [Header("Enemy Interface - Damage Over Time")]
    public List<string> currentDoTNames  = new List<string>();
    public List<int> currentDoTTicks     = new List<int>();

    [Header("Enemy Interface - Enemy Effects")]
    public bool hasBlood = true;
    public float bloodDuration = 1.0f;
    public GameObject bloodEffect;
    public GameObject damageNumber;

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
        amount = (hasFreeze)? amount * enemyData.freezeMult : amount;
        curHealth -= amount;

        if (damageNumber != null && amount > 0)
        {
            GameObject dm = Instantiate(damageNumber);
            dm.transform.position = transform.position;
            dm.GetComponent<DamageNumber>().DoDamage(amount);        
        }

        if (curHealth <= 0) 
        {
            GoldDropSpawner gds = GetComponent<GoldDropSpawner>();
            if (gds) gds.DoGoldDrop();
            KillEnemy();
        }
    }

    public uint GetFillerFlag() { return fillerFlag; }
    public void SetFillerFlag(uint val) { fillerFlag = val; }

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
            case StatusEffectType.Stop:
                Stop stop = statusEffectData as Stop;
                HandleStop(stop);
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

        GameObject particleInstanceBurst, particleInstanceOngoing;
        ParticleSystem psB = null, psO = null;

        if (data.hasBurstPart) {
            particleInstanceBurst = Instantiate(data.burstPart, transform.position, Quaternion.identity, transform);
            particleInstanceBurst.GetComponent<ParticleFollowTransform>().target = transform;
            psB = particleInstanceBurst.GetComponent<ParticleSystem>();
        }

        if (data.hasOngoingPart) {
            particleInstanceOngoing = Instantiate(data.ongoingPart, transform.position, Quaternion.identity, transform);
            particleInstanceOngoing.GetComponent<ParticleFollowTransform>().target = transform;
            psO = particleInstanceOngoing.GetComponent<ParticleSystem>();
        }
        for (int i = 0; i < currentDoTTicks[index] || isDead; i++) {
            TakeDamage(data.tickDamage);
            if (psB != null) {
                psB.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                psB.Play();
            }
            yield return new WaitForSeconds(1f / data.ticksPerSecond);
            index = currentDoTNames.IndexOf(data.name);
        }

        if (psB != null) {
            var emission = psB.emission;
            emission.enabled = false;
        }
        if (psO != null) {
            var emission = psO.emission;
            emission.enabled = false;
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

        GameObject particleInstanceOngoing;
        ParticleSystem psO = null;

        if (numFreeze == 1 && data.hasOngoingPart) {
            particleInstanceOngoing = Instantiate(data.ongoingPart, transform.position, Quaternion.identity, transform);
            psO = particleInstanceOngoing.GetComponent<ParticleSystem>();
        }

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

            if (psO != null) {
                var emission = psO.emission;
                emission.enabled = false;
            }

            hasFreeze = false;
        }
    }

    public virtual void HandleKnockback(Knockback data, Vector3 knockbackOrigin) {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (knockbackOrigin == null) return;
        if (rb != null) {
            Vector3 direction = (transform.position - knockbackOrigin).normalized;
            // direction.y = 0f; leave in if we want 
            rb.AddForce(direction * data.knockbackForce, ForceMode.Force);
            // hasKnockback = true;
        }
    }

    public void HandleStop(Stop data) {
        if (!isStopped) StartCoroutine(StartStop(data));
    }

    private IEnumerator StartStop(Stop data) {
        isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.isStopped = true;
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(data.stopDuration);
        if (rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (agent != null) agent.isStopped = false;
        isStopped = false;
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
