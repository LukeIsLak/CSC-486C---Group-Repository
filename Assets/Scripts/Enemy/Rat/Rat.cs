using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// XXX force rats to have rigidbody and navmeshagent

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

    /*Unlike the states, these let the state manager know when it's time to change the state */
    [Header("State Checkers")]
    public bool isIdle          = false;
    public bool isWandering     = false;
    public bool canWander       = false;
    public bool doneWandering   = false;
    public bool isLeaping       = false;
    public bool canLeap         = true;

    public bool isMoving        = false;

    [Header("Navigation Weight Values")]
    public float navWeight          = 1.5f;
    public float separationWeight   = 2f;
    public float alignmentWeight    = 1f;
    public float cohesionWeight     = 1f;
    public float wanderWeight       = 0.3f;
    public float destStopDist       = 1f;

    [Header("Misc. Variables")]
    public RatStates currentState;
    public bool isRatMaster = false;
    public bool isLoner = false;
    public int ratColonyNum = -1;
    public Vector3 colonyMoveSpot;
    public LayerMask ratMask = ~0;
    public float colonyDist = 5f;

    public float neighbourRadius = 5f;

    public Vector3 curLeapDir;
    public float leapForce = 20f;
    public float leapDuration = 0.5f;
    public float leapCooldown = 3f;

    public float wanderRadius = 4f;

    public float idleDuration = 2f;
    public float minMoveWait = 0.25f;
    public float maxMoveWait = 0.75f;

    public override void initialize() {
        // curHealth = rd.baseHealth * playerData.maxHealth;
        // moveSpeed = rd.baseMoveSpeed * playerData.baseSpeed;

        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();

        RatStateManager env_rsm = FindObjectOfType<RatStateManager>();
        
        rsm = env_rsm;
        rsm.entities.Add(this);
        // XXX set collider radiuses out here
        // XXX set agent parameters here

        nma.updatePosition = false;
        nma.updateRotation = false;
    }

    public void UpdateColonyMove() {
        Debug.Log("Test");
        Vector3 navDir = nma.desiredVelocity;
        Collider[] hits = Physics.OverlapSphere(transform.position, neighbourRadius, ratMask);
    
        Vector3 separation      = Vector3.zero;
        Vector3 alignment       = Vector3.zero;
        Vector3 cohesionCenter  = Vector3.zero;
        int count = 0;

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

        Vector3 alignmentForce  = Vector3.zero;
        Vector3 cohesionForce   = Vector3.zero;

        if (count > 0) {
            alignmentForce = (alignment / count).normalized;
            cohesionForce = ((cohesionCenter / count) - transform.position).normalized;
        }

        Vector3 wander = Random.insideUnitSphere;
        wander.y = 0f;

        Vector3 finalVelocity   = navDir * navWeight
                                + separation * separationWeight
                                + alignmentForce * alignmentWeight
                                + cohesionForce * cohesionWeight
                                + wander * wanderWeight;
        
        Vector3 velocity = Vector3.ClampMagnitude(finalVelocity, moveSpeed);
        transform.position += velocity * Time.deltaTime;

        nma.nextPosition = transform.position;

        if (velocity.sqrMagnitude > 0f) {
            Quaternion rot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
        }
        if (nma != null && IsAgentAtDestination(nma)) {
            FinishWander();
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
        return Vector3.Distance(transform.position, dest) <= destStopDist;
    }

    // public void UpdateAgroApproach() {

    // }

    public void OnTriggerZone(Collider other, TriggerZone zone, TriggerType type) {
        Rat or = other.gameObject.GetComponent<Rat>();
        if (or == null && other.attachedRigidbody != null) {
            /*If the rat is on the rigidbody root, use that instead*/
            or = other.attachedRigidbody.GetComponentInParent<Rat>();
        }
        if (or == null) return;

        switch (type) {
            case TriggerType.Enter:
                if (zone == TriggerZone.Inner) OnTriggerEnterInner(other, or);
                else OnTriggerEnterOuter(other, or);
                return;
            case TriggerType.Stay:
                if (zone == TriggerZone.Inner) OnTriggerStayInner(other, or);
                //XXX outer say eventually?
                return;
            case TriggerType.Exit:
                if (zone == TriggerZone.Inner) OnTriggerEnterInner(other, or);
                else OnTriggerEnterOuter(other, or);
                return;
            default:
                return;
        }
    }

    public void OnTriggerEnterInner(Collider other, Rat or) {
        if (ratColonyNum == or.ratColonyNum && isMoving && or.doneWandering) {
            doneWandering = true;
            isMoving = false;
            if (nma.isOnNavMesh) nma.isStopped = true;
        }
    }

    // XXX maybe make this a OnTriggerStay?
    public void OnTriggerEnterOuter(Collider other, Rat or) {
        if (isLoner) {
            if (or.ratColonyNum != -1) {
                ratColonyNum = or.ratColonyNum;
                isRatMaster = false;
                isLoner = false;
            }
            else {
                // XXX if both rats are null handle this!
                // for simplicity, the one that checks first will be the rat master
                List<Rat> newRatColony = new List<Rat>{this, or};
                rcm.AddRatColony(newRatColony);
                isRatMaster = true;
                isLoner = false;
            }
        }
    }

    public void OnTriggerStayInner(Collider other, Rat or) {
        if (!isWandering) return;
        if (ratColonyNum == or.ratColonyNum && isMoving && or.doneWandering) FinishWander();
    }
    
    public void OnTriggerExitOuter(Collider other, Rat or) {
        if (ratColonyNum != -1 && or.ratColonyNum == ratColonyNum) {
            // XXX maybe I do this with a Collider[] hits = Physics.OverlapSphere(transform.position, neighbourRadius, ratMask);
            foreach(Rat r in rcm.ratColonies[ratColonyNum]) {
                if (r == this) continue;
                if (Vector3.Distance(r.gameObject.transform.position, transform.position) < colonyDist) return;
            }
            
            rcm.RemoveRat(this, ratColonyNum);

            isLoner = true;
            isRatMaster = false;
            ratColonyNum = -1;
        }
    }

    public void FinishWander() {
        doneWandering = true;
        isMoving = false;
        nma.isStopped = true;
        nma.updatePosition = false;
        nma.updateRotation = false;
    }

    public void StartWanderStagger() {
        StartCoroutine(StaggerMove());
    }

    public void StartLeapDuration() {
        StartCoroutine(Leap());
    }

    public void StartIdleDuration() {
        StartCoroutine(Idle());
    }

    private IEnumerator StaggerMove() {
        float randWait = Random.Range(minMoveWait, maxMoveWait);
        yield return new WaitForSeconds(randWait);
        isMoving = true;

    }

    private IEnumerator Leap() {
        isLeaping = true;
        rb.AddForce(curLeapDir * leapForce);
        yield return new WaitForSeconds(leapDuration);
        isLeaping = false;
    }

    private IEnumerator LeapCooldown() {
        yield return new WaitForSeconds(leapCooldown);
        canLeap = true;
    }

    private IEnumerator Idle() {
        isIdle = true;
        yield return new WaitForSeconds(idleDuration);
        isIdle = false;
        canWander = true;
        if (!isLoner && isRatMaster) rcm.DetermineColonyMoveSpot(ratColonyNum);
    }
}
