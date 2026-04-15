using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/DifficultyScaling")]
public class DifficultyScaling : ScriptableObject
{
    public float scalingFraction;

    [Header("Base Parameters")]
    public float minTrapRoomFraction = 0.0f;
    public float maxTrapRoomFraction = 0.5f;
    public int minDungeonInterations = 6;
    public int maxDungeonInterations = 12;
    public float GetTrapRoomFraction()
    {
        // Do calulation and return scaled value
        return Mathf.Lerp(minTrapRoomFraction, maxTrapRoomFraction, scalingFraction);
    }

    public int GetDungeonIterations()
    {
        return Mathf.RoundToInt(Mathf.Lerp(minDungeonInterations, maxDungeonInterations, scalingFraction));
    }

    public float GetSpawnCountMultiplier()
    {
        // Do calulation and return scaled value
        return 1.0f;
    }

    public int GetMinNumberOfWaves()
    {
        // Do calulation and return scaled value
        return 1;
    }

    public int GetMaxNumberOfWaves()
    {
        // Do calulation and return scaled value
        return 1;
    }


}
