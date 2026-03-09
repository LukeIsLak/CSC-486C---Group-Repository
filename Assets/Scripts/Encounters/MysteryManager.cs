using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class MysteryManager : MonoBehaviour
{
    [Header("Data")]
    public RandomContext encRandomContext;
    public EncounterWeightList mysteryWeights;
    public GameEvent ExitEncounterToLayout;
    void Start()
    {
        EncounterInfo selected = SelectEncounter();
        if (!selected) ExitEncounterToLayout.Raise();
        SceneManager.LoadScene(selected.scene);
    }

    private EncounterInfo SelectEncounter()
    {
        Dictionary<EncounterInfo, float> weights = new Dictionary<EncounterInfo, float>();
        float weightSum = 0;

        // Map encounters to weights with dictionary
        foreach (EncounterWeight encProb in mysteryWeights.encounterWeights)
        {
            EncounterInfo enc   = encProb.encounter;
            float weight        = encProb.weight;
            weightSum           += weight;

            if (weights.ContainsKey(enc))
            {
                weights[enc] += weight;
                continue;
            }
            weights[enc] = weight;
        }

        if (weights.Count == 0 || weightSum == 0f)
        {
            Debug.Log("No weights available in list or sum of weights is zero");
            return null;
        }

        // Determine which range r lands on
        float r = encRandomContext.rnd.NextFloat(weightSum);
        foreach (var (key, value) in weights)
        {
            r -= value;
            if (r <= 0f) return key;
        }

        // We shouldn't get here.
        return null;
    }

}
