using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatColonyManager : MonoBehaviour
{
    [SerializeField] public List<List<Rat>> ratColonies = new List<List<Rat>>();
    [SerializeField] public List<Vector3> colonyMoveSpot = new List<Vector3>();
    [SerializeField] public List<bool> ratColoniesMoving = new List<bool>();

    public Rat GetRatMaster(int i) {
        foreach (Rat r in ratColonies[i]) if (r.isRatMaster) return r;
        return null;
    }

    public Vector3 PickWanderSpotOnNavMesh(Vector3 origin, float radius, int maxAttempts = 12) {
        NavMeshHit hit;
        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < maxAttempts; i++) {
            Vector3 cand = origin + Random.insideUnitSphere * radius;
            cand.y = origin.y;

            /*Snap to nearest NavMesh*/
            if (NavMesh.SamplePosition(cand, out hit, Mathf.Max(1f, radius * 0.25f), NavMesh.AllAreas)) {
                /*Verify a complete path exists from the source to this point*/
                if (NavMesh.CalculatePath(origin, hit.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete) {
                    return hit.position;
                }
            }
        }

        /*Fallback to the origin if not able to find a spot on the NavMesh*/
        // XXX in the future fallback to a random spot in the room they are in
        // XXX given radius
            // Pick a random spot in the room
                // if outside radius
                    // take dir, go in the max radius of magnitude in dir
        if (NavMesh.SamplePosition(origin, out hit, radius, UnityEngine.AI.NavMesh.AllAreas)) return hit.position;
        return origin;
    }
    

    public void DetermineColonyMoveSpot(int i) {
        Rat ratMaster = GetRatMaster(i);
        if (ratMaster == null) return; // XXX eventually handle this logic

        /*The moveSpot is determined by the RatMasters move spot*/
        Vector3 centroid = Centroid(i);
        Vector3 ratMPos = ratMaster.gameObject.transform.position;
        Vector3 midPoint = (centroid + ratMPos) * 0.5f;


        Vector3 wanderSpot = PickWanderSpotOnNavMesh(midPoint, ratMaster.rd.wanderRadius);

        foreach(Rat r in ratColonies[i]) SetMoveSpot(r, wanderSpot);
    }

    public void DetermineLonerMoveSpot(Rat r) {
        Vector3 wanderSpot = PickWanderSpotOnNavMesh(r.gameObject.transform.position, r.rd.wanderRadius);
        SetMoveSpot(r, wanderSpot);
    }

    public void SetMoveSpot(Rat r, Vector3 wanderSpot) {
        r.colonyMoveSpot = wanderSpot;

        if (r.nma != null && r.nma.isOnNavMesh) {
            r.nma.SetDestination(wanderSpot);
            r.nma.isStopped = false;
            r.nma.updatePosition = true;
            r.nma.updateRotation = true;
        }
    }

    public Vector3 Centroid(int i) {
        Vector3 c = Vector3.zero;
        foreach (Rat r in ratColonies[i]) c += r.gameObject.transform.position;
        return c /= ratColonies[i].Count;

    }

    public void AddRatToColony(Rat r, int i) {
        ratColonies[i].Add(r);
    }

    public void AddRatColony(List<Rat> newRats) {
        ratColonies.Add(newRats);
        foreach (Rat r in newRats) r.ratColonyNum = ratColonies.Count - 1;
    }

    public void RemoveRat(Rat r, int i) {
        ratColonies[i].Remove(r);
        if (ratColonies.Count <= 1) {
            foreach (Rat or in ratColonies[i]) {
                or.isRatMaster = false;
                or.isLoner = true;
                or.ratColonyNum = -1;
            }
            RemoveAndUpdateColony(i);
        }
        else if (r.isRatMaster) {
            ratColonies[i][0].isRatMaster = true;
        }
    }

    public void RemoveAndUpdateColony(int i) {
        for (int j = i; j < ratColonies.Count; j++) foreach (Rat r in ratColonies[j]) r.ratColonyNum -= 1;
        ratColonies.RemoveAt(i);
    }
}
