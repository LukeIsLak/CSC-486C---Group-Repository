using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatColonyManager : MonoBehaviour
{
    public List<List<Rat>> ratColonies = new List<List<Rat>>();
    public List<bool> ratColoniesMoving = new List<bool>();

    public Rat GetRatMaster(int i) {
        foreach (Rat r in ratColonies[i]) if (r.isRatMaster) return r;
        return null;
    }

    public Vector3 PickWanderSpotOnNavMesh(Vector3 origin, float radius, int maxAttempts = 12) {
        UnityEngine.AI.NavMeshHit hit;
        for (int i = 0; i < maxAttempts; i++) {
            /*Pick a random point in a 2D circle*/
            Vector3 cand = origin + Random.insideUnitSphere * radius;
            cand.y = origin.y;
            
            /*Try to snap that random position to a */
            if (UnityEngine.AI.NavMesh.SamplePosition(cand, out hit, Mathf.Max(1f, radius * 0.25f), UnityEngine.AI.NavMesh.AllAreas)) {
                return hit.position;
            }
        }

        /*Fallback to the origin if not able to find a spot on the NavMesh*/
        // XXX in the future fallback to a random spot in the room they are in
        // XXX given radius
            // Pick a random spot in the room
                // if outside radius
                    // take dir, go in the max radius of magnitude in dir
        if (UnityEngine.AI.NavMesh.SamplePosition(origin, out hit, radius, UnityEngine.AI.NavMesh.AllAreas)) return hit.position;
        return origin;
    }

    public void DetermineColonyMoveSpot(int i) {
        Rat ratMaster = GetRatMaster(i);
        if (ratMaster == null) return; // XXX eventually handle this logic

        /*The moveSpot is determined by the RatMasters move spot*/
        Vector3 centroid = Centroid(i);
        Vector3 ratMPos = ratMaster.gameObject.transform.position;
        Vector3 midPoint = (centroid + ratMPos) * 0.5f;

        //XXX rats within a colony must be able to see another rat without something in the way
        Vector3 wanderSpot = PickWanderSpotOnNavMesh(midPoint, ratMaster.wanderRadius);

        foreach(Rat r in ratColonies[i]) {
            r.colonyMoveSpot = wanderSpot;

            if (r.nma != null && r.nma.isOnNavMesh) {
                r.nma.SetDestination(wanderSpot);
                r.nma.isStopped = false;
            }
        }
    }

    public Vector3 Centroid(int i) {
        Vector3 c = Vector3.zero;
        foreach (Rat r in ratColonies[i]) c += r.gameObject.transform.position;
        return c /= ratColonies[i].Count;

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
                or.ratColonyNum = null;
            }
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
