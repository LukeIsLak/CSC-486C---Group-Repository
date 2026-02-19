using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Require RigidBody maybe?

public class Bat : EnemyInterface
{
    [Header("Bat Base Fields")]
    public Animator anim;
    public Rigidbody rb;
    public BatBaseData bd;
    public Transform playerTransform;
    public BatStateManager bsm;

    [Header("Debug Fields")]
    public bool debug = true;
    public bool debugPoint = true;

    /*Unlike the states, these let the state manager know when it's time to change the state */
    [Header("State Checkers")]
    public bool isMoving            = false;
    public bool isFluttering        = false;
    public bool isPecking           = false;
    public bool isPeckRebounding    = false;
    public bool peckComplete        = false;
    public bool swoopComplete       = false;
    public bool shouldPerch         = false;
    public bool canLeavePerch       = false;
    public bool canAttack           = false;
    public bool isPerched           = false;
    public bool isAttacking         = false;

    [Header("Bat motion")]
    public int currentPathIndex = 0;
    [SerializeField] private List<Vector3> targetPath = new List<Vector3>();

    [Header("Flutter Instance Data")]
    [SerializeField] private float flutterAngleDeg = 0f;
    [SerializeField] private float flutterJitterSeed;
    [SerializeField] private float direction = 1f;

    [Header("Peck Instance Data")]
    public Vector3? peckStartPosition = null;
    public Vector3? interruptPosition = null; // TODO: LK  - This is for the future, should answer the question of what happens if a bat gets hit and doesn't die?

    [Header("Bat Transition Variables")]
    public int attackAmount = 0;
    public BatAttacks? nextAttack;

    [Header("Misc. Variables")]
    [SerializeField] private Vector3 prevLateralOffset = Vector3.zero;
    // [SerializeField] private float attackCooldownTimer = 0f;
    public BatStates currentState;

    /****************************************************/
    /*          Beginning Of Instance Methods           */
    /****************************************************/

    // TODO : LK - This is fragile and volatile, eventually fix
    public override void initialize() {
        curHealth = bd.baseHealth * playerData.maxHealth;
        moveSpeed = bd.baseMoveSpeed * playerData.baseSpeed;

        playerTransform = GameObject.FindWithTag("Player").transform;
        BatStateManager env_bsm = FindObjectOfType<BatStateManager>();
        
        bsm = env_bsm;
        bsm.entities.Add(this);
    }

    public override void KillEnemy() {
        bsm.entities.Remove(this);
        Destroy(this.gameObject);
    }


    /****************************************************/
    /*             End Of Instance Methods              */
    /****************************************************/



    /****************************************************/
    /*     Beginning Of Ceiling / Perching Methods      */
    /****************************************************/

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
        for (int ring = 0; ring <= bd.radialSteps; ring++)
        {
            float t = bd.radialSteps == 0 ? 0f : (float)ring / bd.radialSteps;
            float theta = t * bd.coneAngle; /*Tilt angle in degrees from up*/
            int samples = (ring == 0) ? 1 : Mathf.Max(1, bd.angularSteps * ring); /*ensure more samples for outer rings*/

            for (int s = 0; s < samples; s++)
            {
                float phi = (360f * s) / samples;
                float phiRad = phi * Mathf.Deg2Rad;

                /*Find the tangent axis to rotate up around*/
                Vector3 tangentAxis = (Mathf.Cos(phiRad) * axis1 + Mathf.Sin(phiRad) * axis2).normalized;

                /*Rotate up around this tangent axis by theta degrees to get direction*/
                Vector3 dir = Quaternion.AngleAxis(theta, tangentAxis) * up;

                /*Cast raycast to that position
                    TODO : LK - Note: we would use RayCastAll (or whatever equivalent) here, I choose not to
                    to only allow points in which is inherently visible for now. This can be changed
                    since the plan is to allow the bat to move around objects (in roughly a curve)
                    and theoretically that should allow this behaviour. 
                */
                Physics.Raycast(origin, dir, out RaycastHit hit, bd.maxDistance, 0);
                if (Physics.Raycast(origin, dir, out hit, bd.maxDistance, bd.layerMask) && hit.collider.transform != transform && !hit.collider.transform.IsChildOf(transform))
                {
                    if (Vector3.Angle(hit.normal, -up) <= bd.maxColliderAngle) {
                        hits.Add(hit);
                        if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.red);
                    }
                    else if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.blue);
                }
                else if (debug) Debug.DrawRay(origin, dir * bd.maxDistance, new Color(0f, 1f, 0f, 0.25f));
            }
        }

        return hits;
    }

    private Vector3? findCeilingPoint(List<RaycastHit> hits) {
        if (hits.Count == 0) return null;

        /*Maps a hitScore calculator for each possible perch point*/
        List<KeyValuePair<int, float>> hitScores = hits.Select((x, i) => new KeyValuePair<int, float>(
                                                i,
                                                (Vector3.Angle(x.normal, -transform.up.normalized) * bd.normalWeight) + 
                                                (x.point.y * bd.heightWeight) + 
                                                (Vector3.Angle(transform.position - x.point, transform.up.normalized) * bd.upAngleWeight))).ToList();
        float sum = hitScores.Sum(x => x.Value);
        hitScores.Sort((a, b) => a.Value.CompareTo(b.Value));

        /*Normalize each hitScore*/
        List<KeyValuePair<int, float>> hitPercentages = hitScores.Select(x => new KeyValuePair<int, float>(x.Key, x.Value / sum)).ToList();
        float r = UnityEngine.Random.value;
        float cumulative = 0;

        /*Randomly pick a perch point given normalized weights*/
        for (int i = 0; i < hitPercentages.Count; i++) 
        {
            cumulative += hitPercentages[i].Value;
            if (r <= cumulative) return hits[hitPercentages[i].Key].point;
        }
        return hits[hitPercentages[hitPercentages.Count-1].Key].point;
    }

    public void findPerchSpot() {
        List<RaycastHit> hits = findCeilingMesh();
        Vector3? perchSpot = findCeilingPoint(hits);
        if (perchSpot is Vector3 p) {
            /*Line to decided perch point*/
            Debug.DrawLine(p, transform.position, new Color(1f, 0f, 1f, 1f), 5f);
        
            /*Calculate a cubic bezier path to the perch point*/
            targetPath = calculatePathCube(transform.position, (p + new Vector3(0f, bd.pearchYOffset, 0f)));
            currentPathIndex = 0;
            isMoving = targetPath.Count > 0;

            /*Debug the perch point*/
            if (debug && targetPath.Count > 0) {
                Debug.DrawLine(targetPath[0], transform.position, new Color(0f, 1f, 1f, 1f), 5f);
                for (int i = 1; i < targetPath.Count - 1; i++) {
                    Debug.DrawLine(targetPath[i], targetPath[i+1], new Color(0f, 1f, 1f, 1f), 5f);
                }
            }
        }

        isMoving = true;
    }

    public void UpdatePerching() {
        UpdateMoveSpot(false);

        if (!isMoving) isPerched = true;
    }

    /****************************************************/
    /*        End Of Ceiling / Perching Methods         */
    /****************************************************/


    /****************************************************/
    /*          Beginning Of Helper Methods             */
    /****************************************************/


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
        for (int i = 1; i <= bd.numPathPoint; i++) targetPoints.Add(QuadraticBezierCurvePoint(cur, p1, target, (i / (float)bd.numPathPoint)));
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
        for (int i = 1; i <= bd.numPathPoint; i++) targetPoints.Add(CubicBezierCurvePoint(cur, p1, p2, target, (i / (float)bd.numPathPoint)));

        return targetPoints;
    }

    public void ChangeCurrentSpeed()
    {
        switch (currentState)
        {
            case BatStates.Flutter:
                moveSpeed = bd.baseMoveSpeed * bd.flutterMoveSpeed * playerData.baseSpeed * bd.moveSpeed;
                return;
            case BatStates.PeckAttacking:
                moveSpeed = bd.baseMoveSpeed * bd.peckSpeed * playerData.baseSpeed * bd.moveSpeed;
                return;
            case BatStates.PeckCompleteRebound:
                moveSpeed = bd.baseMoveSpeed * bd.peckReboundSpeed * playerData.baseSpeed * bd.moveSpeed;
                return;
            case BatStates.PeckIncompleteRebound:
                moveSpeed = bd.baseMoveSpeed * bd.peckReboundSpeed * playerData.baseSpeed * bd.moveSpeed;
                return;
            case BatStates.SwoopAttacking:
                moveSpeed = bd.baseMoveSpeed * bd.swoopSpeed * playerData.baseSpeed * bd.moveSpeed;
                return;
            default:
                moveSpeed = bd.baseMoveSpeed * bd.moveSpeed * playerData.baseSpeed;
                return;
        }
    }

    public BatAttacks GetRandomWeightedAttack()
    {
        float totalWeight = bd.weightedAttacks.Sum(w => w.weight);
        float r = UnityEngine.Random.Range(0, totalWeight);
        float cumulative = 0f;
        foreach (var wa in bd.weightedAttacks)
        {
            cumulative += wa.weight;
            if (r < cumulative)
                return wa.attack;
        }
        // fallback (should not happen)
        return bd.weightedAttacks[0].attack;
    }

    void UpdateMoveSpot(bool s) {
        if (isMoving && targetPath.Count > 0) {
            Vector3 target = targetPath[currentPathIndex];
            float step = moveSpeed * speedModifier * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            Vector3 toTarget = target - transform.position;
            if (toTarget.sqrMagnitude > 1e-6f) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget.normalized, Vector3.up), 10f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, target) <= bd.pointTolerance) {
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

    /****************************************************/
    /*             End Of Helper Methods                */
    /****************************************************/


    /****************************************************/
    /*         Beginning Of Flutter Methods             */
    /****************************************************/

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

        /*Change the angle*/
        // TODO: LK - Change flutter to move distance per second instead of angle per second
        flutterAngleDeg += moveSpeed * speedModifier * bd.flutterAngularSpeed * direction * Time.deltaTime;
        if (flutterAngleDeg >= 360f) flutterAngleDeg -= 360f;
        if (flutterAngleDeg <= 0f) flutterAngleDeg += 360f;

        float jitter = (Mathf.PerlinNoise(flutterJitterSeed, Time.time * 0.5f) - 0.5f) * 2f * bd.flutterRadialJitter;
        float radius = Mathf.Max(0.1f, bd.flutterRadius + jitter);

        /*Vertical bob*/
        float vertBob = Mathf.Sin(Time.time * bd.verticalBobSpeed + flutterJitterSeed) * bd.verticalBobAmplitude;

        float angleRad = flutterAngleDeg * Mathf.Deg2Rad;
        Vector3 center = playerTransform.position;
        Vector3 baseDir = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        Vector3 sideDir = new Vector3(-baseDir.z, 0f, baseDir.x); // perpendicular in XZ

        /*Squiggle noise changing with angle and time*/
        float noiseU = angleRad * 0.5f + flutterJitterSeed;
        float noiseV = Time.time * bd.horizontalBobSpeed + flutterJitterSeed;
        float squig = (Mathf.PerlinNoise(noiseU, noiseV) - 0.5f) * 2f; // in [-1,1]

        /* Small radial movementss so circle breathes in and out*/
        float radialMod = squig * (bd.horizontalBobAmplitude * 0.25f);
        Vector3 baseOrbit = baseDir * (radius + radialMod);

        /*Lateral squiggle perpendicular to orbit (produces wavy circle)*/
        Vector3 lateralTarget = sideDir * (squig * bd.horizontalBobAmplitude);
        lateralTarget[1] += bd.flutterBaseHeight;
        float alpha = 1f - Mathf.Exp(-bd.lateralSmoothing * Time.deltaTime);
        Vector3 lateralOffset = Vector3.Lerp(prevLateralOffset, lateralTarget, alpha);
        prevLateralOffset = lateralOffset;

        /*Combine into target position (horizontal orbit + lateral squiggle + vertical bob)*/
        Vector3 targetPos = center + baseOrbit + lateralOffset + new Vector3(0f, vertBob, 0f);

        /*Move toward targetPos*/
        float step = moveSpeed * speedModifier * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        /*Face movement direction*/
        Vector3 toTarget = (targetPos - transform.position);
        if (toTarget.sqrMagnitude > 1e-6f)
        {
            Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, bd.flutterTurnSpeed * Time.deltaTime);
        }
    }

    /****************************************************/
    /*            End Of Flutter Methods                */
    /****************************************************/



    /****************************************************/
    /*          Beginning Of Peck Methods               */
    /****************************************************/

    public void PeckTarget()
    {
        attackAmount -= 1;
        isFluttering = false;

        Vector3 start = transform.position;
        Vector3 end = playerTransform.position;

        /*Make specific bezier cube values for arched curve*/
        Vector3 p1 = new Vector3(start.x, end.y, start.z);
        Vector3 p2 = (p1 + end) * 0.5f;

        /*Find the target via a bezier cube path*/
        targetPath = calculatePathCube(start, end, p1, p2);
        
        currentPathIndex = 0;
        isMoving = true;
        isPecking = true;
        peckStartPosition = start;
    }

    public void UpdatePeck()
    {
        UpdateMoveSpot(false);

        if (!isMoving && isPecking == true && !isPeckRebounding) peckComplete = true;
    }

    public void PeckRebound()
    {
        isPecking = false;
        currentState = BatStates.PeckCompleteRebound;

        float _reboundHeight = Mathf.Max((peckComplete)? peckStartPosition.Value.y + bd.reboundHeight : transform.position.y + bd.reboundIncompleteHeight, bd.minRebountHeight);
        float _reboundDistance = (peckComplete)? bd.reboundDistance : bd.reboundIncompleteDistance;

        Vector3 start = transform.position;
        Vector3 end;
        // TODO: LK - This should eventually be fully implemented
        /*If we completed the peck*/
        if (interruptPosition == null) {
            Vector3 horizontalDir = peckStartPosition.Value - transform.position;
            horizontalDir.y = 0f;
            horizontalDir = horizontalDir.normalized;
            Vector3 reboundTarget = transform.position + horizontalDir * _reboundDistance;
            reboundTarget.y = _reboundHeight;
            end = reboundTarget;
        }
        /*If we did not complete the peck*/
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

    public void UpdatePeckRebound() {
        UpdateMoveSpot(false);

        if (!isMoving && isPecking == true && isPeckRebounding) peckComplete = false;
    }

    /****************************************************/
    /*              End Of Peck Methods                 */
    /****************************************************/



    /****************************************************/
    /*          Beginning Of Swoop Methods              */
    /****************************************************/

    public void SwoopAttack() 
    {

        Vector3 start = transform.position;
        Vector3 playerPos = playerTransform.position;

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

    public void UpdateSwoop()
    {
        UpdateMoveSpot(false);

        if (!isMoving) swoopComplete = true;
    }


    /****************************************************/
    /*             End Of Swoop Methods                 */
    /****************************************************/



    /****************************************************/
    /*          Beginning Of Misc. Movement             */
    /****************************************************/

    // TODO: LK - this steer function... sucks... please improve it!
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

        for (int ring = 0; ring <= bd.steerSteps; ring++)
        {
            float t = bd.steerSteps == 0 ? 0f : (float)ring / bd.steerSteps;
            float theta = t * bd.steerViewAngle; // tilt from forward
            int samples = (ring == 0) ? 1 : Mathf.Max(1, bd.steerAngularSteps * ring);

            for (int s = 0; s < samples; s++)
            {
                float phi = (360f * s) / samples;
                float phiRad = phi * Mathf.Deg2Rad;

                // tangent axis and sample direction
                Vector3 tangentAxis = (Mathf.Cos(phiRad) * axis1 + Mathf.Sin(phiRad) * axis2).normalized;
                Vector3 dir = Quaternion.AngleAxis(theta, tangentAxis) * forward;

                if (Physics.Raycast(origin, dir, out RaycastHit hit, bd.maxSteerDistance, bd.steerLayerMask)
                    && hit.collider != null
                    && hit.collider.transform != transform
                    && !hit.collider.transform.IsChildOf(transform))
                {
                    float hitFactor = 1f - (hit.distance / Mathf.Max(0.0001f, bd.maxSteerDistance)); // 0..1 stronger when close
                    Vector3 away = (origin - hit.point).normalized * hitFactor * bd.steerDistanceWeight;
                    repulsion += away;

                    if (debug) Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.blue, Color.red, hitFactor));
                }
                else if (debug)
                {
                    // visualize free sample rays faintly
                    Debug.DrawRay(origin, dir * Mathf.Min(bd.maxSteerDistance, 1.0f), new Color(0f, 1f, 0f, 0.2f));
                }
            }
        }

        // no obstacles found -> nothing to steer away from
        if (repulsion.sqrMagnitude < 1e-6f) return;

        // build desired direction combining forward intent and repulsion
        Vector3 repulseDir = repulsion.normalized;
        Vector3 desiredDir = (forward + repulseDir * bd.steerDirectionWeight).normalized;

        // move by a step towards the desired direction (clamped by maxSteerDistance)
        Vector3 desiredPos = origin + desiredDir * Mathf.Min(bd.maxSteerDistance, moveSpeed * speedModifier);
        float step = moveSpeed * speedModifier * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, desiredPos, step);

        /* If we want to debug the steer*/
        if (debug) 
        { 
            Debug.DrawRay(origin, repulsion, Color.magenta);
            Debug.DrawLine(origin, desiredPos, Color.cyan);
        }

        /* Rotate parent object to face where we are moving */
        Vector3 toTarget = desiredPos - transform.position;
        if (toTarget.sqrMagnitude > 1e-6f)
        {
            Quaternion look = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
        }
    }

    /****************************************************/
    /*              End Of Misc. Movement               */
    /****************************************************/

    /****************************************************/
    /*                Beginning Of Misc.                */
    /****************************************************/

    // public override void Hit(float damage, float? weight = null, Vector3? colPoint = null) {

    // }

    public void OnTriggerEnter(Collider other) {
        if (isAttacking && other.CompareTag("Player")) {
            Health h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(10f); //TODO: LK - eventually, when we figure out the base values, replace this!
            isAttacking = false;
        }
    }

    /****************************************************/
    /*                  End Of Misc.                    */
    /****************************************************/


    /****************************************************/
    /*       Beginning Of Couroutines / Timers          */
    /****************************************************/

    public void StartAttackCooldown(float delay)
    {
        StartCoroutine(AttackCooldownCoroutine(delay));
    }

    public void StartPerchDuration(float delay)
    {
        StartCoroutine(PerchDuration(delay));
    }

    private IEnumerator PerchDuration(float delay)
    {
        canLeavePerch = false;
        yield return new WaitForSeconds(delay);
        canLeavePerch = true;
    }

    private IEnumerator AttackCooldownCoroutine(float delay)
    {
        canAttack = false;
        yield return new WaitForSeconds(delay);
        canAttack = true;
        if (attackAmount <= 0) shouldPerch = true;
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
    /*                End Of Debuggers                  */
    /****************************************************/



    /****************************************************/
    /*            Beginning Of Debuggers                */
    /****************************************************/

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

    /****************************************************/
    /*                End Of Debuggers                  */
    /****************************************************/
}