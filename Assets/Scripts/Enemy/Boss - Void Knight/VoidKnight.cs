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

    //
    [Header("MovementThreshold")]
    public bool checkPlayerPath = false;
    public float moveThreshold  = 0.5f;  //% of time  boss can move, time with steps
    public bool isStepping      = false;
    private float elapsedTime   = 0f;
    public bool canSeePlayer    = false;

    public VoidKnightStates currentState;
    public bool isInitialized = false;
    public bool isAgro = false;
    //

    public override void initialize() {
        curHealth = vkd.baseHealth * playerData.maxHealth;
        moveSpeed = vkd.baseMoveSpeed * playerData.baseSpeed;

        playerTransform = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        VoidKnightStateMachine env_vksm = FindObjectOfType<VoidKnightStateMachine>();
        vksm = env_vksm;
        vksm.AddEntity(this);
    }

    public void initialize_nma() {
        nma.enabled = true;

        nma.updatePosition = false;
        nma.updateRotation = false;

        isInitialized = true;
    }

    private bool canStep() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 2 * Mathf.PI / moveSpeed) elapsedTime = 0;
        // sin(2*pi+s*p) > (1 - 2*t)
        float sin = Mathf.Sin(2f * Mathf.PI + moveSpeed * elapsedTime);
        float thresh = 1 - 2 * moveThreshold;
        return sin >= thresh;

        // XXX reset after
    }
    private void UpdateMove() {
        Vector3 navDir = nma.desiredVelocity;
        navDir.y = 0f;
        
        Vector3 velocity = Vector3.ClampMagnitude(navDir, moveSpeed);
        Vector3 planarMove = new Vector3(velocity.x, 0f, velocity.z) * Time.deltaTime;
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

    public void UpdateAgroMove() {
        UpdatePlayerPath();
        isStepping = canStep();
        if (isStepping) UpdateMove();
        canSeePlayer = CanSeePlayer();
        // if (nma != null && IsAgentAtDestination(nma)) XXX finish this!
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


    // if the boss can see the player, move towards it
    // boss should remember position of player, but if can't see before attack or for a duration of the wander it loses sight of them

}
