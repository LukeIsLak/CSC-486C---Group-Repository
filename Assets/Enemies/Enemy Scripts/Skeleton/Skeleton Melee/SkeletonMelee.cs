using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(UnityEngine.AI.NavMeshAgent))]
public class SkeletonMelee : EnemyInterface
{
    [Header("Skeleton Meele Base Fields")]
    public Animator anim;
    public Rigidbody rb;
    public SkeletonMeleeBaseData smd;
    public Transform playerTransform;
    public SkeletonMeleeStateMachine smsm;
    public NavMeshAgent nma;

    [Header("State Checkers")]
    public bool startInitialized    = false;
    public bool isInitialized       = false;
    public bool noWallSpot          = false;
    public bool inWallSpawn         = false;
    public bool isIdle              = false;
    public bool canWander           = false;
    public bool doneWandering       = false;
    public bool isAgro              = false;

    public bool isMoving            = false;
    public bool checkPlayerPath     = true;
    
    
    public bool isArmoured          = true;

    public LayerMask wallMask;

    [Header("Misc. Variables")]
    public SkeletonMeleeStates currentState;

    /****************************************************/
    /*          Beginning Of Instance Methods           */
    /****************************************************/

    public override void initialize() {
        curHealth = smd.baseHealth * playerData.maxHealth;
        moveSpeed = smd.baseMoveSpeed * playerData.baseSpeed;

        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        SkeletonMeleeStateMachine env_smsm = FindObjectOfType<SkeletonMeleeStateMachine>();
    }

    public void initialize_nma() {
        if (!startInitialized) StartCoroutine(delay_navmesh());
    }

    private IEnumerator delay_navmesh() {
        startInitialized = true;
        yield return new WaitForSeconds(0.5f);

        if (!nma.isOnNavMesh) {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas)) {
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

    public override void KillEnemy() {
        if (rs != null) rs.RemoveEnemy();
        if(smsm != null) smsm.RemoveEntity(this);
        Destroy(this.gameObject);
    }

    /****************************************************/
    /*             End Of Instance Methods              */
    /****************************************************/


    /****************************************************/
    /*          Beginning Of Helper Methods             */
    /****************************************************/

    public Collider[] FindNearbyWalls(float radius = 1.0f) {
        return Physics.OverlapSphere(transform.position, radius, wallMask);
    }

    public Vector3? FindClosestWallPoint(float radius = 1.0f) {
        Collider[] walls = FindNearbyWalls();
        float minDist = float.MaxValue;
        Vector3? closestPoint = null;

        foreach (var wall in walls)
        {
            Vector3 point = wall.ClosestPoint(transform.position);
            float dist = Vector3.Distance(transform.position, point);
            if (dist < minDist)
            {
                minDist = dist;
                closestPoint = point;
            }
        }
        return closestPoint;
    }

    public Vector3? PickRandomPointInConeAwayFromWall(float wallCheckRadius = 2f, float coneAngle = 60f, float minDist = 1f, float maxDist = 3f) {
        Vector3? wallPoint = FindClosestWallPoint(wallCheckRadius);
        if (wallPoint == null) return null;

        Vector3 dirToWall = (wallPoint.Value - transform.position).normalized;
        Vector3 coneDir = -dirToWall;

        // Pick a random direction within the cone
        float angle = Random.Range(-coneAngle / 2f, coneAngle / 2f);
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        Vector3 randomDir = rot * coneDir;

        float dist = Random.Range(minDist, maxDist);
        Vector3 randomPoint = transform.position + randomDir * dist;

        return randomPoint;
    }

    private void SteerForce(out Vector3 separation, out Vector3 alignment, out Vector3 cohesionCenter, out int count) {
        Collider[] hits = Physics.OverlapSphere(transform.position, smd.neighbourRadius, smd.skeletonMask);
    
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

        Vector3 finalVelocity   = navDir * smd.navWeight
                                + separation * smd.separationWeight
                                + alignmentForce * smd.alignmentWeight
                                + cohesionForce * smd.cohesionWeight
                                + wander * smd.wanderWeight;
        
        Vector3 velocity = Vector3.ClampMagnitude(finalVelocity, moveSpeed);
        Vector3 planarMove = new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime * speedModifier;

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
        return Vector3.Distance(transform.position, dest) <= smd.destStopDist;
    }

    /****************************************************/
    /*             End Of Helper Methods                */
    /****************************************************/



    /****************************************************/
    /*           Beginning of Movement Methods          */
    /****************************************************/

    private void FindSpawnPosition() {
        Vector3? spawnPoint = PickRandomPointInConeAwayFromWall();
        if (spawnPoint != null) {
            transform.position = spawnPoint.Value;
            inWallSpawn = true;
        }
        else noWallSpot = true;

    }

    /****************************************************/
    /*             End Of Movement Methods              */
    /****************************************************/
}
