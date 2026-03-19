using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/ShopItemList")]
public class ShopItemList : ScriptableObject
{
    public List<ShopItem> items;

    public static ShopItem MakeWeightedChoice(List<ShopItem> items, RandomContext randomContext)
    {
        Dictionary<ShopItem, float> weights = new Dictionary<ShopItem, float>();
        float weightSum = 0;

        // Map encounters to weights with dictionary
        foreach (ShopItem acq in items)
        {
            float weight        = acq.acquirable.selectionWeight;
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
