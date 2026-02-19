using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatColonyManager : MonoBehaviour
{
    public List<List<Rat>> ratColonies = new List<List<Rat>>();
    public List<bool> ratColoniesMoving = new List<bool>();

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
