using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

// XXX force rats to have rigidbody and navmeshagent
[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
// [RequireComponent(typeof(Rigidbody))]
public class Rat : EnemyInterface
{
    [Header("Rat Base Fields")]
    public Animator anim;
    public Rigidbody rb;
    public RatBaseData rd;
    public Transform playerTransform;
    public RatColonyManager rcm;
    public RatStateManager rsm;
    public NavMeshAgent nma;
    public StudioEventEmitter audioEmitter;
    // public SphereCollider outerCol;
    // public SphereCollider innerCol;

    /*Unlike the states, these let the state manager know when it's time to change the state */
    [Header("State Checkers")]
    public bool startInitialized    = false;
    public bool isInitialized       = false;
    public bool isIdle              = false;
    public bool isWandering         = false;
    public bool canWander           = false;
    public bool doneWandering       = false;
    public bool isAgro              = false;
    public bool isLeaping           = false;
    public bool canLeap             = true;
    public bool doneLeap            = false;
    public bool waitToMove          = false;
    public bool isMoving            = false;
    public bool checkPlayerPath     = true;


    [Header("Rat Colony Values")]
    public bool isRatMaster = false;
    public bool isLoner     = true;
    public int ratColonyId = -1;
    public Vector3 colonyMoveSpot;

    [Header("Leaping Values")]
    public Vector3 curLeapDir;

    [Header("Misc. Variables")]
    public RatStates currentState;
    public SpriteRenderer sprite;
    

    /****************************************************/
    /*          Beginning Of Instance Methods           */
    /****************************************************/

    public override void initialize() {
        curHealth = rd.baseHealth * playerData.maxHealth;
        moveSpeed = rd.baseMoveSpeed * playerData.baseSpeed;

        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        audioEmitter = GetComponent<StudioEventEmitter>();

        RatStateManager env_rsm     = FindObjectOfType<RatStateManager>();
        RatColonyManager env_rcm    = FindObjectOfType<RatColonyManager>();
        
        rcm = env_rcm;
        rsm = env_rsm;
        rsm.AddEntity(this);
        rcm.AddRatEntity(this);

        // outerCol.radius = rd.colonyDist;
        // innerCol.radius = rd.neighbourStopRadius;
    }

    public void initialize_nma() {
        if (!startInitialized && !isInitialized) StartCoroutine(delay_navmesh());
    }

    private IEnumerator delay_navmesh() {
        startInitialized = true;
        yield return new WaitForSeconds(0.5f);

        if (!nma.isOnNavMesh) {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 15f, NavMesh.AllAreas)) {
                transform.position = hit.position;
            } else {
                Debug.LogWarning($"{gameObject.name}: Could not find NavMesh nearby at {transform.position}!");
                gameObject.SetActive(false);
            }
        }

        nma.enabled = true;

        nma.updatePosition = false;
        nma.updateRotation = false;

        isInitialized = true;
    }

    // public override void KillEnemy() {
    //     if (rs != null) rs.RemoveEnemy();
    //     if (rcm != null) rcm.RemoveRat(this, ratColonyId);
    //     if(rsm != null) rsm.RemoveEntity(this);
    //     Destroy(this.gameObject);
    // }

    public override void KillEnemy() {
        if (isDying) return;
        isDying = true;
        StopAllCoroutines();
        StartCoroutine(DissolveAndDestroy());
    }


    /****************************************************/
    /*             End Of Instance Methods              */
    /****************************************************/



    /****************************************************/
    /*          Beginning Of Helper Methods             */
    /****************************************************/

    private void SteerForce(out Vector3 separation, out Vector3 alignment, out Vector3 cohesionCenter, out int count) {
        Collider[] hits = Physics.OverlapSphere(transform.position, rd.neighbourRadius, rd.ratMask);
    
        separation      = Vector3.zero;
        alignment       = Vector3.zero;
        cohesionCenter  = Vector3.zero;
        count = 0;

        count = 0;
        foreach (Collider h in hits) {
            if (h.transform == transform) continue;
            Rat other = h.gameObject.GetComponent<Rat>();
            if (other == null && h.attachedRigidbody != null) {
                /*If the rat is on the rigidbody root, use that instead*/
                other = h.attachedRigidbody.GetComponentInParent<Rat>();
            }
            if (other == null) continue;

            Vector3 diff = transform.position - other.gameObject.transform.position;

            separation += diff.normalized / Mathf.Max(diff.magnitude, 0.01f);
            alignment += other.rb.velocity;
            cohesionCenter += other.transform.position;
            count ++;
        }
    }

    private void UpdateMove() {
        if (isStopped) return;
        if (!nma.isOnNavMesh) {
            Debug.LogWarning($"{gameObject.name}: Is not on a NavMesh!");
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas)) {
                transform.position = hit.position;
            } else {
                Debug.LogWarning($"{gameObject.name}: Could not find NavMesh nearby at {transform.position}!");
                // Optionally, disable the rat or destroy it here
                gameObject.SetActive(false);
                return;
            }
        }
        nma.nextPosition = transform.position;
        Vector3 navDir = nma.desiredVelocity;
        navDir.y = 0f;

        Vector3 separation, alignment, cohesionCenter;
        int count = 0;

        SteerForce(out separation, out alignment, out cohesionCenter, out count);

        Vector3 alignmentForce  = Vector3.zero;
        Vector3 cohesionForce   = Vector3.zero;

        if (count > 0) {
            alignmentForce = (alignment / count);
            alignmentForce.y = 0f;
            if (alignmentForce.sqrMagnitude > 0f) alignmentForce = alignmentForce.normalized;

            cohesionForce = ((cohesionCenter / count) - transform.position);
            cohesionForce.y = 0f;
            if (cohesionForce.sqrMagnitude > 0f) cohesionForce = cohesionForce.normalized;
        }

        Vector3 wander = Random.insideUnitSphere;
        wander.y = 0f;

        Vector3 finalVelocity   = navDir * rd.navWeight
                                + separation * rd.separationWeight
                                + alignmentForce * rd.alignmentWeight
                                + cohesionForce * rd.cohesionWeight
                                + wander * rd.wanderWeight;
        
        Vector3 velocity = Vector3.ClampMagnitude(finalVelocity, moveSpeed);
        Vector3 planarMove = new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime * speedModifier;

        // if (currentState == RatStates.AgroApproach) {
        //     Debug.Log("NavMeshAgent destination: " + nma.destination);
        //     Debug.Log("NavDir: " + navDir);
        //     Debug.Log("NavDir * weight" + navDir * rd.navWeight);
        //     Debug.Log("Velocity: " + velocity);
        // }
        transform.position += planarMove;
        nma.nextPosition = transform.position;

        Vector3 agentNextPos = nma.nextPosition;
        nma.nextPosition = new Vector3(transform.position.x, agentNextPos.y, transform.position.z);

        if (velocity.sqrMagnitude > 0f) {
            Quaternion rot = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
    }

    private bool IsAgentAtDestination(NavMeshAgent agent) {
        if (agent == null) return false;
        if (agent.pathPending) return false;

        float remaining = agent.remainingDistance;
        if (!float.IsInfinity(remaining) && remaining <= agent.stoppingDistance) {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete || !agent.hasPath) return true;
        }

        Vector3 dest = agent.destination;
        return Vector3.Distance(transform.position, dest) <= rd.destStopDist;
    }

    public void FinishWander() {
        canWander = false;
        doneWandering = true;
        isMoving = false;
        // nma.isStopped = true;
        nma.updatePosition = false;
        nma.updateRotation = false;
    }

    /****************************************************/
    /*             End Of Helper Methods                */
    /****************************************************/



    /****************************************************/
    /*           Beginning of Movement Methods          */
    /****************************************************/

    public void UpdateLonerMove() {
        UpdateMove();
        if (nma != null && IsAgentAtDestination(nma)) FinishWander();
    }
    // TODO: LK - seems redundant for now, if it stays combine with above
    public void UpdateColonyMove() {
        UpdateMove();
        if (nma != null && IsAgentAtDestination(nma)) FinishWander();
    }

    public void UpdateAgroApproach() {
        UpdatePlayerPath();
        UpdateMove();
    }

    /****************************************************/
    /*             End Of Movement Methods              */
    /****************************************************/



    /****************************************************/
    /*              Beginning Of Triggers               */
    /****************************************************/

    // public void OnTriggerZone(Collider other, TriggerZone zone, TriggerType type) {
    //     Rat or = other.gameObject.GetComponent<Rat>();
    //     if (or == null && other.attachedRigidbody != null) {
    //         /*If the rat is on the rigidbody root, use that instead*/
    //         or = other.attachedRigidbody.GetComponentInParent<Rat>();
    //     }
    //     if (or == null) return;

    //     switch (type) {
    //         case TriggerType.Enter:
    //             if (zone == TriggerZone.Inner) OnTriggerEnterInner(other, or);
    //             else OnTriggerEnterOuter(other, or);
    //             return;
    //         case TriggerType.Stay:
    //             if (zone == TriggerZone.Inner) OnTriggerStayInner(other, or);
    //             //XXX outer say eventually?
    //             return;
    //         case TriggerType.Exit:
    //             // if (zone == TriggerZone.Inner) OnTriggerExitInner(other, or);
    //             // else OnTriggerExitOuter(other, or);
    //             if (zone == TriggerZone.Outer) OnTriggerExitOuter(other, or);
    //             return;
    //         default:
    //             return;
    //     }
    // }

    // public void OnTriggerEnterInner(Collider other, Rat or) {
    //     if (ratColonyId == or.ratColonyId && isMoving && or.doneWandering) {
    //         doneWandering = true;
    //         isMoving = false;
    //         // if (nma.isOnNavMesh) nma.isStopped = true;
    //     }
    // }

    // // XXX maybe make this a OnTriggerStay?
    // public void OnTriggerEnterOuter(Collider other, Rat or) {
    //     if (isLoner) {
    //         if (ratColonyId == -1 && or.ratColonyId != -1) {
    //             rcm.AddRatToColony(this, ratColonyId);

    //             canWander = or.canWander;
    //             isWandering = or.isWandering;

    //             if (isInitialized) {
    //                 if (isWandering) {
    //                     colonyMoveSpot = or.colonyMoveSpot;
    //                     nma.SetDestination(colonyMoveSpot);
    //                 }
    //                 // nma.isStopped = !isWandering;
    //                 // nma.updatePosition = isWandering;
    //                 // nma.updateRotation = isWandering;
    //             }
    //         }
    //         else {
    //             //because this is a race condition, whichever one is first will set it
    //             if (ratColonyId != -1) {
    //                 List<Rat> newRatColony = new List<Rat>{this, or};
    //                 rcm.AddRatColony(newRatColony);
    //             }
    //         }
    //     }
    // }

    // public void OnTriggerStayInner(Collider other, Rat or) {
    //     if (!isWandering) return;
    //     if (ratColonyId == or.ratColonyId && isMoving && or.doneWandering) FinishWander();
    // }
    
    // public void OnTriggerExitOuter(Collider other, Rat or) {
    //     if (ratColonyId < 0 || ratColonyId >= rcm.ratColonies.Count) return;
    //     if (or.ratColonyId != ratColonyId) return;

    //     // Check if any other colony member is still within range
    //     RatColony colony = rcm.ratColonies[ratColonyId];
    //     foreach (Rat r in colony.members) {
    //         if (r == this) continue;
    //         if (r == null) continue;
    //         if (Vector3.Distance(r.gameObject.transform.position, transform.position) < rd.colonyDist) return;
    //     }

    //     rcm.RemoveRat(this, ratColonyId);
    // }

    private void OnCollisionEnter(Collision other) {
        if (!isDying && isLeaping && other.gameObject.CompareTag("Player")) {
            Health h = other.gameObject.GetComponent<Health>();
            if (h != null) h.TakeDamage(rd.damage);
            isLeaping = false; // XXX should use another thing here
        }
    }

    /****************************************************/
    /*                End Of Triggers                   */
    /****************************************************/



    /****************************************************/
    /*       Beginning Of Couroutines / Timers          */
    /****************************************************/

    public void StartWanderStagger() {
        if (!waitToMove) StartCoroutine(StaggerMove());
    }

    public void StartIdleDuration() {
        StartCoroutine(Idle());
    }

    public void UpdatePlayerPath() {
        if (checkPlayerPath) StartCoroutine(DelayCalcPlayerPath());
    }

    public void StartLeap() {
        Vector3 toPlayer    = playerTransform.position - transform.position;
        Vector3 horizontal  = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (horizontal.sqrMagnitude == 0f) return;

        // Preserve y velocity
        float velocityY     = rb.velocity.y;

        transform.forward   = horizontal.normalized;

        if (toPlayer.magnitude < rd.leapRadius && canLeap)
        {
            curLeapDir = horizontal;
            curLeapDir.y = rd.leapYIncrease;
            StartCoroutine(Leap());
            return;
        }
    }

    public void StartLeapCooldown() {
        StartCoroutine(LeapCooldown());
    }

    private IEnumerator StaggerMove() {
        waitToMove = true;
        float randWait = Random.Range(rd.minMoveWait, rd.maxMoveWait);
        yield return new WaitForSeconds(randWait);
        isMoving = true;
        waitToMove = false;

    }

    private IEnumerator Leap() {
        anim.SetBool("isAttack", true);

        isLeaping = true;
        canLeap = false;
        // nma.isStopped = true;
        nma.updatePosition = false;
        nma.updateRotation = false;

        rb.AddForce(curLeapDir * rd.leapForce, ForceMode.Impulse);

        yield return new WaitForSeconds(rd.leapDuration);

        isLeaping = false;
        doneLeap = true;

        nma.nextPosition = transform.position;

        anim.SetBool("isAttack", false);
    }

    private IEnumerator LeapCooldown() {
        doneLeap = false;
        yield return new WaitForSeconds(rd.leapCooldown);
        canLeap = true;
    }

    private IEnumerator Idle() {
        isIdle = true;
        yield return new WaitForSeconds(rd.idleDuration);
        isIdle = false;
        canWander = true;
        if (isLoner) {
            float addIdle = Random.Range(rd.minAddIdleWait, rd.maxAddIdleWait);
            yield return new WaitForSeconds(addIdle);
            rcm.DetermineLonerMoveSpot(this);
        }
        else if (isRatMaster) {
            /*The idea here is too provide the colony some time and to stagger the colonies*/
            float addIdle = Random.Range(rd.minAddIdleWait, rd.maxAddIdleWait);
            yield return new WaitForSeconds(addIdle);
            rcm.DetermineColonyMoveSpot(ratColonyId);
        }
    }

    private IEnumerator DelayCalcPlayerPath() {
        checkPlayerPath = false;
        Vector3 ratPos = transform.position;
        Vector3 plaPos = playerTransform.position;
        ratPos.y = 0;
        plaPos.y = 0;
        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer > rd.preferredRange + rd.rangeTolerance) {
            isMoving = true;
            nma.SetDestination(playerTransform.position);
        }
        else if (distToPlayer < rd.preferredRange - rd.rangeTolerance) {
            isMoving = true;
            Vector3 dirAway = (transform.position - playerTransform.position).normalized;
            Vector3 targetPos = transform.position + dirAway * (rd.preferredRange - distToPlayer + 0.5f);
            nma.SetDestination(targetPos);
            UpdateMove();
        }
        else {
            isMoving = false;
            nma.ResetPath();
        }
        yield return new WaitForSeconds(rd.checkPlayerUpdate);
        checkPlayerPath = true;
    }

    private IEnumerator DissolveAndDestroy() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent) agent.isStopped = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.velocity = Vector3.zero;
        if (anim != null) anim.enabled = false;

        float dissolveTime = rd.dissolveTime;
        float timer = 0f;
        Material mat = sprite.material;

        while (timer < dissolveTime) {
            float t = timer / dissolveTime;
            mat.SetFloat("_fade", 1f - t);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.SetFloat("_fade", 0f);
        if (rs != null) rs.RemoveEnemy();
        if (rcm != null) rcm.RemoveRat(this, ratColonyId);
        if(rsm != null) rsm.RemoveEntity(this);

        Destroy(gameObject);
    }

    /****************************************************/
    /*           End Of Couroutines / Timers            */
    /****************************************************/



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

}
