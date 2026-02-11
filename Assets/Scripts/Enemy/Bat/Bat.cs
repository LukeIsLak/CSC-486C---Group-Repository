using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Require RigidBody maybe?

public class Bat : EnemyInterface
{
    [Header("Bat Base Field")]
    public Animator anim;

    [Header("Ceiling Location Possibilities")]
    public int radialSteps = 1;
    public int angularSteps = 10;
    public float coneAngle = 45f;
    public float maxDistance = 10f;
    public float maxColliderAngle = 45f;
    public LayerMask layerMask = ~0;
    public bool debug = true;
    public bool debugPoint = true;

    [Header("Ceiling Position Weights")]
    public float normalWeight = 1f;
    public float heightWeight = 1f;
    public float upAngleWeight = 1f;

    [Header("Bat motion")]
    public int numPathPoint = 4;
    public float moveSpeed = 3f;
    public float pointTolerance = 0.05f;
    public int currentPathIndex = 0;
    public bool isMoving = false;
    private List<Vector3> targetPath = new List<Vector3>();

    // Flutter behaviour parameters
    [Header("Flutter (around player)")]
    public Transform playerTransform;               // assign in inspector or at runtime
    public bool isFluttering = false;
    public float flutterRadius = 3f;                // average orbit radius
    public float flutterAngularSpeed = 90f;         // degrees per second
    public float flutterRadialJitter = 0.5f;        // random variation in radius
    public float verticalBobAmplitude = 0.4f;       // vertical bob amount
    public float verticalBobSpeed = 2f;             // vertical bob speed
    public float horizontalBobAmplitude = 0.4f;     // horizontal bob amount
    public float horizontalBobSpeed = 2f;
    public float flutterMoveSpeed = 4f;             // movement speed while fluttering
    public float flutterTurnSpeed = 5f;             // rotation smoothing
    public float flutterBaseHeight = 2f;
    [SerializeField]
    private float flutterAngleDeg = 0f;
    [SerializeField]
    private float flutterJitterSeed;
    [SerializeField]
    private float direction = 1f;

    // Smoothing for horizontal squiggle to reduce spikiness (higher = smoother)
    [Header("Flutter Noise Smoothing")]
    public float lateralSmoothing = 8f;

    [Header("Bat ")]

    [Header("Bat Peck Variables")]
    public bool testPeck = false;
    public bool isPecking = false;
    public bool isPeckRebounding = false;
    public bool peckComplete = false;
    public float peckSpeed = 5f;
    public float peckReboundSpeed = 10f;
    public Vector3? peckStartPosition = null;
    public Vector3? interruptPosition = null;
    public float reboundHeight = 1f;
    public float reboundIncompleteHeight = 1f;
    public float minRebountHeight = 1f;
    public float reboundDistance = 1f;
    public float reboundIncompleteDistance = 1f; // incomplete as in attack doesn't finish

    [Header("Bat Swoop Variables")]
    public bool swoopComplete = false;
    public float swoopSpeed = 5f;

    [Header("Steering Controls")]
    public int steerSteps = 1;
    public int steerAngularSteps = 10;
    public float steerViewAngle = 45f;
    public float maxSteerDistance = 10f;
    public float steerDistanceWeight = 2f;
    public float steerDirectionWeight = 1f;
    public LayerMask steerLayerMask = ~0;

    private Vector3 prevLateralOffset = Vector3.zero;

    [Header("Bat Transition Variables")]
    public float playerSearchDistance = 5;
    public bool isPerched = false;
    public int attackAmount = 0;
    public int minAttackAmount = 1;
    public int maxAttackAmount = 3;
    public bool canAttack = false;
    public BatAttacks? nextAttack;

    [Header("Attack Timing")]
    public bool isAttacking = false;
    public float attackCooldown = 2f;
    private float attackCooldownTimer = 0f;

    public List<WeightedAttack> weightedAttacks = new List<WeightedAttack>
    {
        new WeightedAttack(BatAttacks.PeckAttack, 2f),
        new WeightedAttack(BatAttacks.SwoopAttack, 1f)
    };


    public BatStates currentState;

    private List<RaycastHit> findCeilingMesh() {
        List<RaycastHit> hits = new List<RaycastHit>();
        Vector3 origin = transform.position;
        Vector3 up = transform.up.normalized;

        Vector3 axis1 = transform.forward;
        if (Mathf.Abs(Vector3.Dot(axis1.normalized, up)) > 0.99f)
        {
            axis1 = transform.right;
        }
        Vector3 axis2 = Vector3.Cross(up, axis1).normalized;
        axis1 = Vector3.Cross(axis2, up).normalized;
        for (int ring = 0; ring <= radialSteps; ring++)
        {
            float t = radialSteps == 0 ? 0f : (float)ring / radialSteps;
            float theta = t * coneAngle; /*Tilt angle in degrees from up*/
            int samples = (ring == 0) ? 1 : Mathf.Max(1, angularSteps * ring); /*ensure more samples for outer rings*/

            for (int s = 0; s < samples; s++)
            {
                float phi = (360f * s) / samples;
                float phiRad = phi * Mathf.Deg2Rad;

                /*Find the tangent axis to rotate up around*/
                Vector3 tangentAxis = (Mathf.Cos(phiRad) * axis1 + Mathf.Sin(phiRad) * axis2).normalized;

                /*Rotate up around this tangent axis by theta degrees to get direction*/
                Vector3 dir = Quaternion.AngleAxis(theta, tangentAxis) * up;

                /*Cast raycast to that position
                    XXX Note: we would use RayCastAll (or whatever equivalent) here, I choose not to
                    to only allow points in which is inherently visible for now. This can be changed
                    since the plan is to allow the bat to move around objects (in roughly a curve)
                    and theoretically that should allow this behaviour. 
                */
                Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, 0);
                if (Physics.Raycast(origin, dir, out hit, maxDistance, layerMask) && hit.collider.transform != transform && !hit.collider.transform.IsChildOf(transform))
                {
                    if (Vector3.Angle(hit.normal, -up) <= maxColliderAngle) {
                        hits.Add(hit);
                        if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.red);
                    }
                    else if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.blue);
                }
                else if (debug) Debug.DrawRay(origin, dir * maxDistance, new Color(0f, 1f, 0f, 0.25f));
            }
        }

        return hits;
    }

    /*Can return nullable value*/
    private Vector3? findCeilingPoint(List<RaycastHit> hits) {
        /*
            Idea:
                Score function based on
                    - normal of hit point
                    - height / y-axis of hit
                    - angle of the original cast to up
        */
        if (hits.Count == 0) return null; 
        List<KeyValuePair<int, float>> hitScores = hits.Select((x, i) => new KeyValuePair<int, float>(
                                                i,
                                                (Vector3.Angle(x.normal, -transform.up.normalized) * normalWeight) + 
                                                (x.point.y * heightWeight) + 
                                                (Vector3.Angle(transform.position - x.point, transform.up.normalized) * upAngleWeight))).ToList();
        float sum = hitScores.Sum(x => x.Value);
        hitScores.Sort((a, b) => a.Value.CompareTo(b.Value));
        List<KeyValuePair<int, float>> hitPercentages = hitScores.Select(x => new KeyValuePair<int, float>(x.Key, x.Value / sum)).ToList();
        float r = UnityEngine.Random.value;
        float cumulative = 0;
        for (int i = 0; i < hitPercentages.Count; i++) 
        {
            cumulative += hitPercentages[i].Value;
            if (r <= cumulative) return hits[hitPercentages[i].Key].point;
        }
        return hits[hitPercentages[hitPercentages.Count-1].Key].point;
    }

    private void OnDrawGizmos()
    {
        if (!debug && !debugPoint) return;
        try {
            List<RaycastHit> hits = findCeilingMesh();
            foreach (var hit in hits)
            {
                /*Where does it collide?*/
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, 0.0125f);

                /*What is the normal at that collision point?*/
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal * 0.25f);
            }
        }
        catch {}
    }

    public static Vector3 ReflectAcrossAxis(Vector3 v, Vector3 axis) {
        if (axis.sqrMagnitude < 1e-12f) return v; // no axis -> identity
        Vector3 u = axis.normalized;
        return 2f * Vector3.Dot(v, u) * u - v;
    }

    private static Vector3 QuadraticBezierCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        float u = 1f - t;
        return (u * u * p0) + 
               (2f * u * t * p1) +
               (t * t * p2);
    }

    /*t is between [0, 1]*/
    private static Vector3 CubicBezierCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        float u = 1f - t;
        return (u * u * u * p0) + 
               (3f * u * u * t * p1) +
               (3f * u * t * t * p2) +
               (t * t * t * p3);
    }

    private List<Vector3> calculatePathQuad(Vector3 cur, Vector3 target, Vector3 p1) {
        List<Vector3> targetPoints = new List<Vector3>();
        for (int i = 1; i <= numPathPoint; i++) targetPoints.Add(QuadraticBezierCurvePoint(cur, p1, target, (i / (float)numPathPoint)));
        return targetPoints;
    }
    
    private List<Vector3> calculatePathCube(Vector3 cur, Vector3 target, Vector3? _p1 = null, Vector3? _p2 = null) {
        Vector3 dir = (target - cur);
        Vector3 normal = Vector3.Cross(dir, Vector3.up).normalized;

        /*maybe set the 0.25 and 0.75 as random or parameter?*/
        float d1 = dir.magnitude * 0.25f; // quarter way
        float d2 = dir.magnitude * 0.75f; // 3-quarter way

        /*hyper parameter??*/
        Vector3 p1 = (_p1 != null)? _p1.Value : cur + dir * 0.25f + normal * Random.Range(-2f, 2f);
        Vector3 p2 = (_p2 != null)? _p2.Value :cur + dir * 0.25f + normal * Random.Range(-2f, 2f);

        List<Vector3> targetPoints = new List<Vector3>();
        for (int i = 1; i <= numPathPoint; i++) targetPoints.Add(CubicBezierCurvePoint(cur, p1, p2, target, (i / (float)numPathPoint)));

        return targetPoints;
    }

    // Returns the movement speed based on the current state
    // XXX this is oddly inefficient, make this a enter state call
    private float GetCurrentSpeed()
    {
        switch (currentState)
        {
            case BatStates.Flutter:
                return flutterMoveSpeed;
            case BatStates.PeckAttacking:
                return peckSpeed;
            case BatStates.PeckCompleteRebound:
                return peckReboundSpeed;
            case BatStates.PeckIncompleteRebound:
                return peckReboundSpeed;
            case BatStates.SwoopAttacking:
                return swoopSpeed;
            default:
                return moveSpeed;
        }
    }

    public void StartFlutter(Transform player)
    {
        if (player == null) return;
        playerTransform = player;
        isFluttering = true;
        direction = (UnityEngine.Random.Range(0f, 1f) < 0.5) ? -1f : 1f;
        flutterAngleDeg = UnityEngine.Random.Range(0f, 360f);
        flutterJitterSeed = UnityEngine.Random.value * 100f;
        // optionally stop path-following
        isMoving = false;
        currentState = BatStates.Flutter;
        nextAttack = GetRandomWeightedAttack();
    }

    public void UpdateFlutter()
    {
        if (!isFluttering || playerTransform == null) return;

        // Advance angle
        flutterAngleDeg += flutterAngularSpeed * direction * Time.deltaTime;
        if (flutterAngleDeg >= 360f) flutterAngleDeg -= 360f;
        if (flutterAngleDeg <= 0f) flutterAngleDeg += 360f;

        // Smooth radial jitter
        float jitter = (Mathf.PerlinNoise(flutterJitterSeed, Time.time * 0.5f) - 0.5f) * 2f * flutterRadialJitter;
        float radius = Mathf.Max(0.1f, flutterRadius + jitter);

        // Vertical bob
        float vertBob = Mathf.Sin(Time.time * verticalBobSpeed + flutterJitterSeed) * verticalBobAmplitude;

        float angleRad = flutterAngleDeg * Mathf.Deg2Rad;
        Vector3 center = playerTransform.position;

        // Unit vectors for orbit
        Vector3 baseDir = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        Vector3 sideDir = new Vector3(-baseDir.z, 0f, baseDir.x); // perpendicular in XZ

        // Perlin-based squiggle evolving with angle and time
        float noiseU = angleRad * 0.5f + flutterJitterSeed;
        float noiseV = Time.time * horizontalBobSpeed + flutterJitterSeed;
        float squig = (Mathf.PerlinNoise(noiseU, noiseV) - 0.5f) * 2f; // in [-1,1]

        // Small radial modulation so circle breathes
        float radialMod = squig * (horizontalBobAmplitude * 0.25f);
        Vector3 baseOrbit = baseDir * (radius + radialMod);

        // Lateral squiggle perpendicular to orbit (produces wavy circle)
        Vector3 lateralTarget = sideDir * (squig * horizontalBobAmplitude);
        lateralTarget[1] += flutterBaseHeight;
        // exponential smoothing: alpha in (0,1) per-frame derived from smoothing rate
        float alpha = 1f - Mathf.Exp(-lateralSmoothing * Time.deltaTime);
        Vector3 lateralOffset = Vector3.Lerp(prevLateralOffset, lateralTarget, alpha);
        prevLateralOffset = lateralOffset;

        // Combine into target position (horizontal orbit + lateral squiggle + vertical bob)
        Vector3 targetPos = center + baseOrbit + lateralOffset + new Vector3(0f, vertBob, 0f);

        // Move smoothly toward targetPos
        float step = flutterMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Smoothly face movement direction
        Vector3 toTarget = (targetPos - transform.position);
        if (toTarget.sqrMagnitude > 1e-6f)
        {
            Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, flutterTurnSpeed * Time.deltaTime);
        }
    }


    public void PeckTarget() {
        attackAmount -= 1;
        isFluttering = false;

        Vector3 start = transform.position;
        Vector3 end = playerTransform.position;

        Vector3 p1 = new Vector3(start.x, end.y, start.z);
        Vector3 p2 = (p1 + end) * 0.5f;

        targetPath = calculatePathCube(start, end, p1, p2);
        
        currentPathIndex = 0;
        isMoving = true;
        isPecking = true;
        peckStartPosition = start;
    }

    public void PeckRebound() {
        isPecking = false;
        currentState = BatStates.PeckCompleteRebound;

        float _reboundHeight = Mathf.Max((peckComplete)? peckStartPosition.Value.y + reboundHeight : transform.position.y + reboundIncompleteHeight, minRebountHeight);
        float _reboundDistance = (peckComplete)? reboundDistance : reboundIncompleteDistance;

        Vector3 start = transform.position;
        Vector3 end;
        if (interruptPosition == null) {
            Vector3 horizontalDir = peckStartPosition.Value - transform.position;
            horizontalDir.y = 0f;
            horizontalDir = horizontalDir.normalized;
            Vector3 reboundTarget = transform.position + horizontalDir * _reboundDistance;
            reboundTarget.y = _reboundHeight;
            end = reboundTarget;
        }
        else {
            Vector3 awayDir = transform.position - interruptPosition.Value;
            awayDir.y = 0f;
            awayDir = awayDir.normalized;
            Vector3 reboundTarget = transform.position + awayDir * _reboundDistance;
            reboundTarget.y = _reboundHeight;
            end = reboundTarget;
        }

        Vector3 p1 = new Vector3(start.x, end.y, start.z);
        Vector3 p2 = (p1 + end) * 0.5f;
        
        targetPath = calculatePathCube(start, end);

        currentPathIndex = 0;
        isMoving = true;
        isPeckRebounding = true;
        peckStartPosition = null;
    }

    public void SwoopAttack() {
        // isFluttering = false;

        Vector3 start = transform.position;
        Vector3 playerPos = playerTransform.position;

        // // Direction from bat to player (horizontal only)
        Vector3 swoopDir = (playerPos - start);
        swoopDir.y = 0f;
        swoopDir = swoopDir.normalized;

        float swoopDistance = 5f;
        Vector3 end = playerPos + swoopDir * swoopDistance;
        end.y = playerPos.y;

        Vector3 p1 = 2 * playerPos - (start + end) / 2;

        targetPath = calculatePathQuad(start, end, p1);

        currentPathIndex = 0;
        isMoving = true;
        isPecking = false;
        isPeckRebounding = false;
        peckStartPosition = start;
    }

    // XXX this steer function... sucks... please improve it!
    public void steer()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward.normalized;

        // build orthonormal sampling frame around forward
        Vector3 axis1 = transform.forward;
        if (Mathf.Abs(Vector3.Dot(axis1.normalized, forward)) > 0.99f) axis1 = transform.right;
        Vector3 axis2 = Vector3.Cross(forward, axis1).normalized;
        axis1 = Vector3.Cross(axis2, forward).normalized;

        // accumulate repulsion from hits
        Vector3 repulsion = Vector3.zero;

        for (int ring = 0; ring <= steerSteps; ring++)
        {
            float t = steerSteps == 0 ? 0f : (float)ring / steerSteps;
            float theta = t * steerViewAngle; // tilt from forward
            int samples = (ring == 0) ? 1 : Mathf.Max(1, steerAngularSteps * ring);

            for (int s = 0; s < samples; s++)
            {
                float phi = (360f * s) / samples;
                float phiRad = phi * Mathf.Deg2Rad;

                // tangent axis and sample direction
                Vector3 tangentAxis = (Mathf.Cos(phiRad) * axis1 + Mathf.Sin(phiRad) * axis2).normalized;
                Vector3 dir = Quaternion.AngleAxis(theta, tangentAxis) * forward;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, maxSteerDistance, steerLayerMask)
                    && hit.collider != null
                    && hit.collider.transform != transform
                    && !hit.collider.transform.IsChildOf(transform))
                {
                    float hitFactor = 1f - (hit.distance / Mathf.Max(0.0001f, maxSteerDistance)); // 0..1 stronger when close
                    Vector3 away = (origin - hit.point).normalized * hitFactor * steerDistanceWeight;
                    repulsion += away;

                    if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.blue, Color.red, hitFactor));
                }
                else if (debug)
                {
                    // visualize free sample rays faintly
                    Debug.DrawRay(origin, dir * Mathf.Min(maxSteerDistance, 1.0f), new Color(0f, 1f, 0f, 0.2f));
                }
            }
        }

        // no obstacles found -> nothing to steer away from
        if (repulsion.sqrMagnitude < 1e-6f) return;

        // build desired direction combining forward intent and repulsion
        Vector3 repulseDir = repulsion.normalized;
        Vector3 desiredDir = (forward + repulseDir * steerDirectionWeight).normalized;

        // move by a step towards the desired direction (clamped by maxSteerDistance)
        Vector3 desiredPos = origin + desiredDir * Mathf.Min(maxSteerDistance, moveSpeed);
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, step);

        if (debug) Debug.DrawRay(origin, repulsion, Color.magenta);
        if (debug) Debug.DrawLine(origin, desiredPos, Color.cyan);

        // rotate to face movement
        Vector3 toTarget = desiredPos - transform.position;
        if (toTarget.sqrMagnitude > 1e-6f)
        {
            Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }
    }

    public void findPerchSpot() {
        List<RaycastHit> hits = findCeilingMesh();
        Vector3? perchSpot = findCeilingPoint(hits);
        if (perchSpot is Vector3 p) {
            /*Line to decided perch point*/
            Debug.DrawLine(p, transform.position, new Color(1f, 0f, 1f, 1f), 5f);
        
            targetPath = calculatePathCube(transform.position, p);
            currentPathIndex = 0;
            isMoving = targetPath.Count > 0;
            if (debug && targetPath.Count > 0) {
                Debug.DrawLine(targetPath[0], transform.position, new Color(0f, 1f, 1f, 1f), 5f);
                for (int i = 1; i < targetPath.Count - 1; i++) {
                    Debug.DrawLine(targetPath[i], targetPath[i+1], new Color(0f, 1f, 1f, 1f), 5f);
                }
            }
        }

        isMoving = true;
    }

    public void findWanderPath() {
        //iterate until end spot is away from the player distance wise or line of sight is broken with the player

        bool endCondition = false;

        while (!endCondition) {
            
        }
    }

    public void UpdatePerching() {
        UpdateMoveSpot(false);

        if (!isMoving) isPerched = true;
    }

    public void UpdatePeck() {
        UpdateMoveSpot(false);

        if (!isMoving && isPecking == true && !isPeckRebounding) peckComplete = true;
    }

    public void UpdateSwoop() {
        UpdateMoveSpot(false);

        if (!isMoving) swoopComplete = true;
    }

    public void UpdatePeckRebound() {
        UpdateMoveSpot(false);

        if (!isMoving && isPecking == true && isPeckRebounding) peckComplete = false;
    }


    void UpdateMoveSpot(bool s) {
        if (isMoving && targetPath.Count > 0) {
            Vector3 target = targetPath[currentPathIndex];
            float step = GetCurrentSpeed() * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            Vector3 toTarget = target - transform.position;
            if (toTarget.sqrMagnitude > 1e-6f) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget.normalized, Vector3.up), 10f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, target) <= pointTolerance) {
                currentPathIndex++;
                if (currentPathIndex >= targetPath.Count) {
                    if (isPecking) peckComplete = true;
                    isMoving = false;
                    transform.rotation = Quaternion.identity;
                    // transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);
                }
            }
        }

        if (s) steer();
    }

    public BatAttacks GetRandomWeightedAttack()
    {
        float totalWeight = weightedAttacks.Sum(w => w.weight);
        float r = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0f;
        foreach (var wa in weightedAttacks)
        {
            cumulative += wa.weight;
            if (r < cumulative)
                return wa.attack;
        }
        // fallback (should not happen)
        return weightedAttacks[0].attack;
    }

    public void StartAttackCooldown(float delay)
    {
        StartCoroutine(AttackCooldownCoroutine(delay));
    }

    private IEnumerator AttackCooldownCoroutine(float delay)
    {
        canAttack = false;
        yield return new WaitForSeconds(delay);
        canAttack = true;
    }

    public override void Hit(float damage, float? weight = null, Vector3? colPoint = null) {

    }


    /******************************/
    /*    Collision Condition     */ 
    /******************************/

    public void OnTriggerEnter(Collider other) {
        if (isAttacking && other.CompareTag("Player")) {
            Health h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(10f); // XXX eventually 
            isAttacking = false;
        }
    }
}
