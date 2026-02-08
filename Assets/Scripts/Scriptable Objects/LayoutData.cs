using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/LayoutData")]
public class LayoutData : ScriptableObject
{
    public int              depth,
                            maxWidth,
                            randomSeed;
    public bool             useSeed,
                            shouldGenerate,
                            shouldDoProgress;
    public EncounterType    currentEncounter;
}
