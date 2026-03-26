using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RatColony {
    public int id;
    public List<Rat> members = new List<Rat>();
    public Vector3 colonyMoveSpot;
    public bool ratColonyMoving = false;
    public Rat ratMaster;
}

public class RatColonyManager : MonoBehaviour
{
    [SerializeField] public List<RatColony> ratColonies = new List<RatColony>();
    [SerializeField] public List<Rat> lonerRats = new List<Rat>();
    [SerializeField] public List<Rat> ratEntities = new List<Rat>();
    private int nextColonyId = 0;

    void Update() {
        UpdateColoniesAndLoners();
        UpdateColonyWanderSpot();
    }

    public void UpdateColoniesAndLoners() {
        //Prioritize loners joining existing colonies
        List<Rat> joined = new List<Rat>();
        foreach (Rat loner in lonerRats) {
            bool joinedColony = false;
            for (int c = 0; c < ratColonies.Count; c++) {
                RatColony colony = ratColonies[c];
                foreach (Rat member in colony.members) {
                    if (Vector3.Distance(loner.transform.position, member.transform.position) <= loner.rd.colonyJoinRadius) {
                        AddRatToColony(loner, c);
                        joined.Add(loner);
                        joinedColony = true;
                        break;
                    }
                }
                if (joinedColony) break;
            }
        }
        foreach (Rat r in joined) lonerRats.Remove(r);

        //Handle remaining loners forming new colonies
        List<Rat> processed = new List<Rat>();
        for (int i = 0; i < lonerRats.Count; i++) {
            Rat r1 = lonerRats[i];
            if (processed.Contains(r1)) continue;
            List<Rat> nearbyLoners = new List<Rat> { r1 };
            for (int j = i + 1; j < lonerRats.Count; j++) {
                Rat r2 = lonerRats[j];
                if (processed.Contains(r2)) continue;
                float dist = Vector3.Distance(r1.transform.position, r2.transform.position);
                if (dist <= r1.rd.colonyJoinRadius) {
                    nearbyLoners.Add(r2);
                }
            }
            if (nearbyLoners.Count > 1) {
                // Form a new colony
                CreateColony(nearbyLoners);
                foreach (Rat r in nearbyLoners) processed.Add(r);
                foreach (Rat r in nearbyLoners) lonerRats.Remove(r);
            }
        }

        //Handle colony members leaving if too far from colony
        for (int c = ratColonies.Count - 1; c >= 0; c--) {
            RatColony colony = ratColonies[c];
            Vector3 centroid = Centroid(c);
            List<Rat> toRemove = new List<Rat>();
            foreach (Rat r in colony.members) {
                if (Vector3.Distance(r.transform.position, centroid) > r.rd.colonyLeaveRadius) {
                    toRemove.Add(r);
                }
            }
            foreach (Rat r in toRemove) {
                SetRatToLoner(r);
                lonerRats.Add(r);
            }
        }
    }

    public void UpdateColonyWanderSpot() {
        foreach (RatColony rc in ratColonies) {
            for (int i = 0; i < rc.members.Count; i++) {
                for (int j = i+1; j < rc.members.Count; j++) {
                    Rat r1 = rc.members[i];
                    Rat r2 = rc.members[j];

                    // XXX check the states here
                    if (Vector3.Distance(r1.transform.position, r2.transform.position) < r1.rd.colonyFinishRadius) {
                        if (r1.doneWandering && r2.isMoving) r2.FinishWander();
                        if (r2.doneWandering && r1.isMoving) r1.FinishWander();
                    }
                }
            }
        }
    }

    public void AddRatEntity(Rat r) {
        ratEntities.Add(r);
        lonerRats.Add(r);
    }

    public void RemoveRatEntity(Rat r) {
        ratEntities.Remove(r);
    }

    public RatColony CreateColony(List<Rat> initialMembers) {
        RatColony colony = new RatColony { id = nextColonyId++ };
        HashSet<Rat> uniqueMembers = new HashSet<Rat>(initialMembers); // Remove duplicates
        colony.members.AddRange(uniqueMembers);
        ratColonies.Add(colony);
        foreach (Rat r in uniqueMembers) {
            r.isLoner = false;
            r.ratColonyId = colony.id;
        }
        colony.members[0].isRatMaster = true;
        colony.ratMaster = colony.members[0];
        return colony;
    }

    public Rat GetRatMaster(int i) {
        if (i < 0 || i >= ratColonies.Count) return null;
        foreach (Rat r in ratColonies[i].members) if (r.isRatMaster) return r;
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
        if (i < 0 || i >= ratColonies.Count) return;
        Rat ratMaster = GetRatMaster(i);
        if (ratMaster == null) return; // XXX eventually handle this logic

        /*The moveSpot is determined by the RatMasters move spot*/
        Vector3 centroid = Centroid(i);
        Vector3 ratMPos = ratMaster.gameObject.transform.position;
        Vector3 midPoint = (centroid + ratMPos) * 0.5f;


        Vector3 wanderSpot = PickWanderSpotOnNavMesh(midPoint, ratMaster.rd.wanderRadius);

        foreach(Rat r in ratColonies[i].members) SetMoveSpot(r, wanderSpot);
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
            // r.nma.updatePosition = true;
            // r.nma.updateRotation = true;
        }
    }

    public Vector3 Centroid(int i) {
        Vector3 c = Vector3.zero;
        foreach (Rat r in ratColonies[i].members)
        {
            if (r != null) c += r.gameObject.transform.position;
        }
        return c /= ratColonies[i].members.Count;
    }

    public void SetRatToLoner(Rat r) {
        if (!r.isLoner) {
            if (r.ratColonyId >= 0) RemoveRat(r, r.ratColonyId);
            r.isLoner = true;
            r.ratColonyId = -1;
        }
    }

    public void AddRatToColony(Rat r, int colonyId) {
        if (colonyId < 0 || colonyId >= ratColonies.Count) return;
        if (r.isLoner || r.ratColonyId != colonyId) {
            SetRatToLoner(r); // Remove from previous colonies
            if (!ratColonies[colonyId].members.Contains(r)) {
                ratColonies[colonyId].members.Add(r);
            }
            r.ratColonyId = colonyId;
            r.isLoner = false;
            r.isRatMaster = false;
        }
    }

    public void AddRatColony(List<Rat> newRats) {
        List<Rat> filteredRats = new List<Rat>();
        foreach (Rat r in newRats) if (r.ratColonyId == -1) filteredRats.Add(r);
        if (filteredRats.Count == 0) return; // No valid rats to add
        CreateColony(filteredRats);
    }

    public void RemoveRat(Rat r, int i) {
        if (r != null) RemoveRatEntity(r);
        if (r.isLoner || r.ratColonyId < 0) {
            lonerRats.Remove(r);
            return;
        }
        ratColonies[i].members.Remove(r);
        if (ratColonies[i].members.Count < 2) {
            foreach (Rat or in ratColonies[i].members) {
                or.isRatMaster     = false;
                or.isLoner         = true;
                or.ratColonyId    = -1;
            }

            RemoveAndUpdateColony(i);
        }
        else if (r.isRatMaster) {
            /*Set next rat as the rat master*/
            ratColonies[i].members[0].isRatMaster = true;
        }
    }

    public void RemoveAndUpdateColony(int i) {
        /*Adjust the index of each colony after the removed one*/
        for (int j = i+1; j < ratColonies.Count; j++) foreach (Rat r in ratColonies[j].members) if (!r.isLoner) r.ratColonyId -= 1;
        ratColonies.RemoveAt(i);
        nextColonyId--;
    }
}
