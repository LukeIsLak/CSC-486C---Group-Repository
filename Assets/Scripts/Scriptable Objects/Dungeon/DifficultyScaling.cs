using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/DifficultyScaling")]
public class DifficultyScaling : ScriptableObject
{
    [Header("Trap Parameters")]
    public float trapRoomFraction = 0.2f;
    public int minWaves;
    public int maxWaves;

}
