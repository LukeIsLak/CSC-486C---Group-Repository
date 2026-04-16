using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VoidKnight : EnemyInterface
{

    [Header("Void Knight Base Field")]
    public Animator anim;
    public Rigidbody rb;
    public VoidKnightBaseData vkd;
    public Transform playerTransform;
    public VoidKnightStateMachine vksm;
    public NavMeshAgent nma;

    public float currentMoveSpeed;

    [Header("MovementThreshold")]
    public bool checkPlayerPath = false;
    public float moveDistMult   = 2.0f;
    public float moveThreshold  = 0.5f;  //% of time  boss can move, time with steps
    public bool isStepping      = false;
    private float elapsedTime   = 0f;
    public bool canSeePlayer    = false;

    public float currentMoveDistMult;

    public VoidKnightStates currentState;
    public bool isInitialized = false;
    public bool isAgro = false;
    public float minAgroToAttackTime = 0.5f;
    public float maxAgroToAttackTime = 4.0f;

    public bool isAttacking = false;
    public bool delayCombo1 = false;
    public float comboMoveSpeed = 10f;
    public float comboMoveMultDist = 10f;

    public float combo1PlayerRange = 10f;
    public float comboInBetweenTime = 0.3f;

    // XXX add cast move speed?
    // XXX cast start delay
    // XXX on death raise event

    public int minNumFireball = 5;
    public int maxNumFireball = 15;
    public float minDurFireballCast = 0.1f;
    public float maxDurFireballCast = 0.3f;
    public GameObject fireball;
    public GameObject divineJudgement;

    public int minNumDivineJudgement = 3;
    public int maxNumDivineJudgement = 9;
    public float minDurDivineJudgementCast = 0.1f;
    public float maxDurDivineJudgementCast = 0.15f;

    // XXX I want to change out of this soon
    // XXX lol I already have this mb
    public float minAttackWait = 0.5f;
    public float maxAttackWait = 3f;
    public bool canAttack = false;
    public bool canDamage = false;
    public VoidKnightAttacks? nextAttack = null;


    public bool inAttackCombo1pt1 = false;
    public bool inAttackCombo1pt2 = false;
    public bool inAttackCombo1pt3 = false;
    public Transform castSpawn;
    public float castDelay = 0.4f;

    public GameEvent OnDefeat;
    public SpriteRenderer sprite;

    public List<VoidKnightWeightedAttacks> weightedAttacks;

    void Awake() {
        weightedAttacks = new List<VoidKnightWeightedAttacks> {
            new VoidKnightWeightedAttacks(VoidKnightAttacks.AttackCombo1, 2f, () => 
                Vector3.Distance(transform.position, playerTransform.position) <= combo1PlayerRange),
            new VoidKnightWeightedAttacks(VoidKnightAttacks.AttackFireBall, 1f, () => true),
            new VoidKnightWeightedAttacks(VoidKnightAttacks.AttackDivineJudgement, 1f, () => true)
        };

        initialize();
    }

    public void ChangeMoveSpeed() {
        switch (currentState) {
            case VoidKnightStates.AttackCombo1:
                currentMoveSpeed = comboMoveSpeed;
                currentMoveDistMult = comboMoveMultDist;
                break;
            default:
                currentMoveSpeed = moveSpeed;
                currentMoveDistMult = moveDistMult;
                break;
        }
    }

    public VoidKnightAttacks GetNextAttack() {
        List<VoidKnightWeightedAttacks> possibleAttacks = weightedAttacks.Where(wa => wa.condition == null || wa.condition()).ToList();


        float totalWeight = possibleAttacks.Sum(wa => wa.weight);
        float r = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0f;
        foreach (var wa in possibleAttacks)
        {
            cumulative += wa.weight;
            if (r < cumulative)
                return wa.attack;
        }
        // fallback (should not happen)
        return possibleAttacks[0].attack;
    }
    //

    public override void initialize() {
        curHealth = vkd.baseHealth * playerData.maxHealth;
        moveSpeed = vkd.baseMoveSpeed * playerData.baseSpeed;
        currentMoveSpeed = moveSpeed;
        currentMoveDistMult = moveDistMult;

        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        VoidKnightStateMachine env_vksm = FindObjectOfType<VoidKnightStateMachine>();
        vksm = env_vksm;
        vksm.AddEntity(this);

        initialize_nma();
    }

    public void initialize_nma() {
        nma.enabled = true;

        nma.updatePosition = false;
        nma.updateRotation = false;

        isInitialized = true;
    }

    private bool canStep() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 2 * Mathf.PI / currentMoveSpeed) elapsedTime = 0;
        // sin(2*pi+s*p) > (1 - 2*t)
        float sin = Mathf.Sin(2f * Mathf.PI + currentMoveSpeed * elapsedTime);
        float thresh = 1 - 2 * moveThreshold;
        return sin >= thresh;

        // XXX reset after
    }
    private void UpdateMove() {
        Vector3 navDir = nma.desiredVelocity;
        navDir.y = 0f;
        
        Vector3 velocity = Vector3.ClampMagnitude(navDir, currentMoveSpeed);
        Vector3 planarMove = new Vector3(velocity.x, 0f, velocity.z) * currentMoveDistMult * Time.deltaTime * speedModifier;
        transform.position += planarMove;

        Vector3 agentNextPos = nma.nextPosition;
        nma.nextPosition = new Vector3(transform.position.x, agentNextPos.y, transform.position.z);

        if (velocity.sqrMagnitude > 0f) {
            Quaternion rot = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    private bool IsAgentAtDestination(UnityEngine.AI.NavMeshAgent agent) {
        if (agent == null) return false;
        if (agent.pathPending) return false;

        float remaining = agent.remainingDistance;
        if (!float.IsInfinity(remaining) && remaining <= agent.stoppingDistance) {
            if (agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete || !agent.hasPath) return true;
        }

        Vector3 dest = agent.destination;
        return Vector3.Distance(transform.position, dest) <= vkd.destStopDist;
    }

    public void UpdateAgroMove(bool stepTillFalse = false) {
        UpdatePlayerPath();
        if (!stepTillFalse) isStepping = canStep();
        else if (isStepping) isStepping = canStep();
        if (isStepping) UpdateMove();
        canSeePlayer = CanSeePlayer();
        // if (nma != null && IsAgentAtDestination(nma)) XXX finish this!
    }

    public void OnTriggerEnter(Collider other) {
        if (!isDying && isAttacking && other.CompareTag("Player")) {
            print("Test");
            Health h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(vkd.damage); //TODO: LK - eventually, when we figure out the base values, replace this!
            isAttacking = false;
        }
    }

    private bool CanSeePlayer() {
        if (playerTransform == null) return false; //XXX we should never get here!

        Vector3 from = transform.position + Vector3.up * 0.5f;
        Vector3 toPlayer = playerTransform.position - from;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > vkd.sightDistance) return false;
        float angleToPlayer = Vector3.Dot(this.transform.position, playerTransform.position);
        if (angleToPlayer > vkd.sightAngle * 0.5f) return false;

        RaycastHit hit;
        if (Physics.Raycast(from, toPlayer.normalized, out hit, distanceToPlayer, vkd.viewMask)) {
            // consider it visible if the first hit is the player (or tagged "Player")
            if (hit.transform == playerTransform || hit.collider.CompareTag("Player")) return true;
            return false;
        }

        return true;
    }

    public void UpdatePlayerPath() {
        if (checkPlayerPath) StartCoroutine(DelayCalcPlayerPath());
    }

    private IEnumerator DelayCalcPlayerPath() {
        checkPlayerPath = false;
        nma.SetDestination(playerTransform.position);
        yield return new WaitForSeconds(vkd.checkPlayerUpdate);
        checkPlayerPath = true;
    }

    public void WaitToAttack() {
        canAttack = false;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack() {
        float delay = UnityEngine.Random.Range(minAgroToAttackTime, maxAgroToAttackTime);
        yield return new WaitForSeconds(delay);

        nextAttack = GetNextAttack();
        canAttack = true;
    }

    /******** Combo Attack ************/
    public void forceStep() {
        float c = (1 - 2 * moveThreshold);
        elapsedTime = (Mathf.Asin(c) / currentMoveSpeed) + (2 * Mathf.PI / currentMoveSpeed) + 0.001f;
        isStepping = true;
    }
    public void StartAttackCombo1(int step) {
        isAttacking = true;
        switch (step) {
            case 1: 
                inAttackCombo1pt1 = true;
                break;
            case 2:
                inAttackCombo1pt2 = true;
                break;
            default:
                inAttackCombo1pt3 = true;
                break;
        }
        forceStep();
        checkPlayerPath = true;
        UpdatePlayerPath();
        //XXX make sure I can see the player cause I want no obstacles
    }

    public void UpdateAttackCombo1(int step) {
        if (isStepping) isStepping = canStep();
        if (isStepping) UpdateAgroMove(true);
        if (!isStepping && !delayCombo1) StartCoroutine(WaitAttackCombo1(step));
    }

    private IEnumerator WaitAttackCombo1(int step) {
        delayCombo1 = true;
        yield return new WaitForSeconds(comboInBetweenTime);
        switch (step) {
                case 1: 
                    inAttackCombo1pt1 = false;
                    break;
                case 2:
                    inAttackCombo1pt2 = false;
                    break;
                default:
                    inAttackCombo1pt3 = false;
                    break;
        }
        delayCombo1 = false;
        isAttacking = false;
    }


    /******** Fireball Attack ********/

    public void CastFireBall() {
        int count = UnityEngine.Random.Range(minNumFireball, maxNumFireball);
        if (!isAttacking) StartCoroutine(FireballAttack(count));
    }

    private IEnumerator FireballAttack(int count) {
        if (!isAttacking) {
            isAttacking = true;
            yield return new WaitForSeconds(castDelay);
            for (int i = 0; i < count; i++) {
                GameObject fb = Instantiate(fireball, castSpawn.position, Quaternion.identity);
                Vector3 direction = (playerTransform.position - castSpawn.position).normalized;
                fb.GetComponent<VoidKnightFireball>().Initialize(direction);

                float cooldown = UnityEngine.Random.Range(minDurFireballCast, maxDurFireballCast);
                yield return new WaitForSeconds(cooldown);
            }

            isAttacking = false;
        }
    }


    /******** Divine Judgement Attack ********/

    public void CastDivineJudgement() {
        int count = UnityEngine.Random.Range(minNumDivineJudgement, maxNumDivineJudgement);
        if (!isAttacking) StartCoroutine(DivineJudgementAttack(count));
    }

    private IEnumerator DivineJudgementAttack(int count) {
        if (!isAttacking) {
            isAttacking = true;
            yield return new WaitForSeconds(castDelay);
            for (int i = 0; i < count; i++) {
                GameObject fb = Instantiate(divineJudgement, playerTransform.position, Quaternion.identity);

                float cooldown = UnityEngine.Random.Range(minDurDivineJudgementCast, maxDurDivineJudgementCast);
                yield return new WaitForSeconds(cooldown);
            }

            isAttacking = false;
        }
    }
    

    // public override void KillEnemy() {
    //     if (rs != null) rs.RemoveEnemy();
    //     if (OnDefeat != null) OnDefeat.Raise();
    //     Destroy(this.gameObject);
    // }

    public override void KillEnemy() {
        if (isDying) return;
        isDying = true;
        StopAllCoroutines();
        StartCoroutine(DissolveAndDestroy());
    }

    /****************************************************/
    /*         Beginning Of Event Listeners             */
    /****************************************************/

    public void ApplySpeedModifier() 
    {
        speedModifier = enemyEffects.getEnemySpeedModifier();
    }

    /****************************************************/
    /*             End Of Event Listeners               */
    /****************************************************/

    // XXX since the divine judgement leaves the player vulnerable maybe is the player
    // is within range add a slash attack?



    // if the boss can see the player, move towards it
    // boss should remember position of player, but if can't see before attack or for a duration of the wander it loses sight of them

    private IEnumerator DissolveAndDestroy() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent) agent.isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;
        if (anim != null) anim.enabled = false;

        float dissolveTime = vkd.dissolveTime;
        float timer = 0f;
        Material mat = sprite.material;

        while (timer < dissolveTime) {
            float t = timer / dissolveTime;
            mat.SetFloat("_fade", 1f - t);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_fade", 0f);
        // if (rs != null) rs.RemoveEnemy();
        if (vksm != null) vksm.RemoveEntity(this);
        if (OnDefeat != null) OnDefeat.Raise();
        Destroy(gameObject);
    }
}
