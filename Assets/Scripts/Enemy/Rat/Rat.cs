using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rat : EnemyInterface
{
    [Header("Rat Base Fields")]
    public Animator anim;
    public Rigidbody rb;
    public RatBaseData rd;
    public Transform playerTransform;
    public RatColonyManager rcm;
    public NavMeshAgent nma;

    /*Unlike the states, these let the state manager know when it's time to change the state */
    [Header("State Checkers")]
    public bool isLeaping   = false;
    public bool canLeap     = true;

    [Header("Navigation Weight Values")]
    public float navWeight          = 1.5f;
    public float separationWeight   = 2f;
    public float alignmentWeight    = 1f;
    public float cohesionWeight     = 1f;
    public float wanderWeight       = 0.3f;

    [Header("Misc. Variables")]
    public RatStates currentState;
    public bool isRatMaster = false;
    public bool isLoner = false;
    public int? ratColonyNum = null;
    public Vector3? colonyMoveSpot = null;
    public LayerMask ratMask = ~0;
    public float colonyDist = 5f;

    public float neighbourRadius = 5f;

    public Vector3 curLeapDir;
    public float leapForce = 20f;
    public float leapDuration = 0.5f;
    public float leapCooldown = 3f;


    public override void initialize() {
        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();

        nma.updatePosition = false;
        nma.updateRotation = false;
    }

    public void FindWanderSpot() {

    }

    public void UpdateColonyMove() {
        Vector3 navDir = nma.desiredVelocity;
        Collider[] hits = Physics.OverlapSphere(transform.position, neighbourRadius, ratMask);
    
        Vector3 separation      = Vector3.zero;
        Vector3 alignment       = Vector3.zero;
        Vector3 cohesionCenter  = Vector3.zero;
        int count = 0;

        foreach (Collider h in hits) {
            if (h.transform == transform) continue;
            Rat other = h.GetComponent<Rat>();
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
    }

    public void OnTriggerEnter(Collider other) {
        Rat or = other.gameObject.GetComponent<Rat>();
        if (isLoner && or != null) {
            if (or.ratColonyNum != null) {
                ratColonyNum = or.ratColonyNum.Value;
                isLoner = false;
            }
            else {
                // XXX if both rats are null handle this!
                // for simplicity, the one that checks first will be the rat master
                List<Rat> newRatColony = new List<Rat>{this, or};
                rcm.AddRatColony(newRatColony);
                isRatMaster = true;
            }
        }
    }
    
    public void OnTriggerExit(Collider other) {
        Rat or = other.gameObject.GetComponent<Rat>();
        if (or != null && ratColonyNum != null && or.ratColonyNum.Value == ratColonyNum.Value) {
            // XXX maybe I do this with a Collider[] hits = Physics.OverlapSphere(transform.position, neighbourRadius, ratMask);
            foreach(Rat r in rcm.ratColonies[ratColonyNum.Value]) {
                if (r == this) continue;
                if (Vector3.Distance(r.gameObject.transform.position, transform.position) < colonyDist) return;
            }
            
            rcm.RemoveRat(this, ratColonyNum.Value);

            isLoner = true;
            isRatMaster = false;
            ratColonyNum = null;
        }
    }

    public void StartLeapDuration() {
        StartCoroutine(Leap());
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
}
