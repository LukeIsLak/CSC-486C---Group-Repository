using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/AcquirableList")]
public class AcquirableList : ScriptableObject
{
    public List<Acquirable> items;

    public static Acquirable MakeWeightedChoice(List<Acquirable> items, RandomContext randomContext)
    {
        Dictionary<Acquirable, float> weights = new Dictionary<Acquirable, float>();
        float weightSum = 0;

        // Map encounters to weights with dictionary
        foreach (Acquirable acq in items)
        {
            float weight        = acq.selectionWeight;
            weightSum           += weight;

            if (weights.ContainsKey(acq))
            {
                weights[acq] += weight;
                continue;
            }
            weights[acq] = weight;
        }

        if (weights.Count == 0 || weightSum == 0f)
        {
            Debug.Log("No weights available in list or sum of weights is zero");
            return null;
        }

        // Determine which range r lands on
        float r = randomContext.NextFloat(weightSum);
        foreach (var (key, value) in weights)
        {
            r -= value;
            if (r <= 0f) return key;
        }

        // We shouldn't get here.
        return null;
    }
}
