using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/DungeonGen")]
public class DungeonData : ScriptableObject
{
    [Header("Generation Parameters")]
    public int              dungeonPoolSize,
                            dungeonIters,
                            dungeonItersPerSpecial,
                            dungeonSeed;
    public bool             useSeed;
}
