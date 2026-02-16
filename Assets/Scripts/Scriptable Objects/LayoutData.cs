using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/LayoutData")]
public class LayoutData : ScriptableObject
{
    public int              depth,
                            maxWidth,
                            randomSeed;
    public float            complexity;
    public bool             useSeed,
                            shouldGenerate,
                            shouldDoProgress;
    public EncounterInfo    currentEncounter;
    public List<int>        completedIndices = new List<int>(); // For regeneration!

    [Header("Fog Of War")]
    public GameEvent        FogUpdated;
    public int              layersRevealed = 2;

    public void AddRevealed(int n)
    {
        layersRevealed += n;
        FogUpdated.Raise();
    }


    [Header("Visualization")]
    public int              layerDistance = 4;
    public int              encounterSep = 4;

}
