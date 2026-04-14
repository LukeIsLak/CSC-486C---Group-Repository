using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/DifficultyScaling")]
public class DifficultyScaling : ScriptableObject
{
    public float scalingFraction;

    public float GetTrapRoomFraction()
    {
        // Do calulation and return scaled value
        return 0.5f;
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
