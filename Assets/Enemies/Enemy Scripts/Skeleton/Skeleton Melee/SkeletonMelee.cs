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
        env_smsm.AddEntity(this);

        initialize_nma();
    }

    public void initialize_nma() {
        if (!startInitialized) StartCoroutine(delay_navmesh());
        else isInitialized = true;
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

    public Collider[] FindNearbyWalls(float radius = 10.0f) {
        return Physics.OverlapSphere(transform.position, radius, wallMask);
    }

    public Vector3? FindClosestWallPoint(float radius = 10.0f) {
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

    public Vector3? PickRandomPointInConeAwayFromWall(float wallCheckRadius = 20f, float coneAngle = 60f, float minDist = 0.5f, float maxDist = 10f) {
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
            SkeletonMelee other = h.gameObject.GetComponent<SkeletonMelee>();
            if (other == null && h.attachedRigidbody != null) {
                /*If the rat is on the rigidbody root, use that instead*/
                other = h.attachedRigidbody.GetComponentInParent<SkeletonMelee>();
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

    private bool IsGrounded() {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, LayerMask.GetMask("Default", "Ground"));
    }

    /****************************************************/
    /*             End Of Helper Methods                */
    /****************************************************/



    /****************************************************/
    /*           Beginning of Movement Methods          */
    /****************************************************/

    private void FindSpawnPosition() {
        Vector3? wallPoint = FindClosestWallPoint();
        if (wallPoint != null) {
            nma.enabled = false;
            Vector3 dirFromWall = (transform.position - wallPoint.Value).normalized;
            float offset = 0.1f;
            Vector3 spawnPos = wallPoint.Value + dirFromWall * offset;
            transform.position = spawnPos;

            // Raycast from the wall point toward the skeleton to get the wall normal
            RaycastHit hit;
            Vector3 rayDir = (transform.position - wallPoint.Value).normalized;
            if (Physics.Raycast(wallPoint.Value - rayDir * 0.01f, rayDir, out hit, 1f, wallMask)) {
                transform.rotation = Quaternion.LookRotation(dirFromWall, hit.normal);
            } else {
                transform.rotation = Quaternion.LookRotation(dirFromWall, Vector3.up);
            }

            inWallSpawn = true;
        } else {
            noWallSpot = true;
        }
    }

    public void GetOffWallAtAngle(float distance = 1.0f, float angleDegrees = 60f, float jumpForce = 7f) {
        if (!inWallSpawn) return;

        Vector3 wallNormal = transform.up;

        /*Get direction of the proposed landing direction*/
        Vector3 randomPerp = Vector3.Cross(wallNormal, Random.onUnitSphere).normalized;
        Quaternion rot = Quaternion.AngleAxis(angleDegrees, randomPerp);
        Vector3 offWallDir = rot * (-wallNormal);
        Vector3 targetPos = transform.position + offWallDir * distance;

        /*Get velocity needed to reach proposed landing spot*/
        Vector3 toTarget = targetPos - transform.position;
        float time = Mathf.Max(0.5f, toTarget.magnitude / jumpForce);
        Vector3 velocity = new Vector3(toTarget.x / time, jumpForce, toTarget.z / time);

        rb.velocity = velocity;

        transform.rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0, velocity.z), Vector3.up);
        inWallSpawn = false;

        StartCoroutine(ReenableNavMeshAfterLanding());
    }

    public void UpdateAgroApproach() {
        UpdatePlayerPath();
        UpdateMove();
    }

    /****************************************************/
    /*             End Of Movement Methods              */
    /****************************************************/



    /****************************************************/
    /*             Beginning of State Methods           */
    /****************************************************/

    public void InitializeSpawn() {
        if (smd.wallSpawnChance >= UnityEngine.Random.Range(0, 1)) FindSpawnPosition();
    }

    /****************************************************/
    /*               End Of State Methods               */
    /****************************************************/

    //XXX add bar here for ienumerators
    //XXX set ignore layers on the spawn states not in prefab

    public void UpdatePlayerPath() {
        if (checkPlayerPath) StartCoroutine(DelayCalcPlayerPath());
    }

    private IEnumerator DelayCalcPlayerPath() {
        checkPlayerPath = false;
        nma.SetDestination(playerTransform.position);
        yield return new WaitForSeconds(smd.checkPlayerUpdate);
        checkPlayerPath = true;
    }


    private IEnumerator ReenableNavMeshAfterLanding() {
        while (!IsGrounded())
            yield return null;

        rb.isKinematic = true;
        nma.enabled = true;
    }
}
