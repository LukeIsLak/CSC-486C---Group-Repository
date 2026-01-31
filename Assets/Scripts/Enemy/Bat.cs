using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Bat : EnemyInterface
{

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
    private bool isMoving = false;
    private List<Vector3> targetPath = new List<Vector3>();


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

    /*t is between [0, 1]*/
    private Vector3 CubicBezierCurvePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        float u = 1f - t;
        return (u * u * u * p0) + 
               (3f * u * u * t * p1) +
               (3f * u * t * t * p2) +
               (t * t * t * p3);
    }
    private List<Vector3> calculatePath(Vector3 cur, Vector3 target) {
        Vector3 dir = (target - cur);
        Vector3 normal = Vector3.Cross(dir, Vector3.up).normalized;

        /*maybe set the 0.25 and 0.75 as random or parameter?*/
        float d1 = dir.magnitude * 0.25f; // quarter way
        float d2 = dir.magnitude * 0.75f; // 3-quarter way

        /*hyper parameter??*/
        Vector3 p1 = cur + dir * 0.25f + normal * Random.Range(-2f, 2f);
        Vector3 p2 = cur + dir * 0.25f + normal * Random.Range(-2f, 2f);

        print(cur);
        print(p1);
        print(p2);
        print(target);
        List<Vector3> targetPoints = new List<Vector3>();
        for (int i = 1; i <= numPathPoint; i++) targetPoints.Add(CubicBezierCurvePoint(cur, p1, p2, target, (i / (float)numPathPoint)));

        return targetPoints;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            List<RaycastHit> hits = findCeilingMesh();
            
            Vector3? perchSpot = findCeilingPoint(hits);
            if (perchSpot is Vector3 p) {
                /*Line to decided perch point*/
                Debug.DrawLine(p, transform.position, new Color(1f, 0f, 1f, 1f), 5f);
            
                targetPath = calculatePath(transform.position, p);
                currentPathIndex = 0;
                isMoving = targetPath.Count > 0;
                if (debug && targetPath.Count > 0) {
                    Debug.DrawLine(targetPath[0], transform.position, new Color(0f, 1f, 1f, 1f), 5f);
                    for (int i = 1; i < targetPath.Count - 1; i++) {
                        Debug.DrawLine(targetPath[i], targetPath[i+1], new Color(0f, 1f, 1f, 1f), 5f);
                    }
                }
            }
        }

        if (isMoving && targetPath.Count > 0) {
            Vector3 target = targetPath[currentPathIndex];
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);

            Vector3 toTarget = target - transform.position;
            if (toTarget.sqrMagnitude > 1e-6f) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTarget.normalized, Vector3.up), 10f * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, target) <= pointTolerance) {
                currentPathIndex++;
                if (currentPathIndex >= targetPath.Count) {
                    isMoving = false;
                    transform.rotation = Quaternion.identity;
                    transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);
                }
            }
        }
    }
}
