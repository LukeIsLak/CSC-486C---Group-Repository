using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/LayoutData")]
public class LayoutData : ScriptableObject
{
    [Header("Generation Parameters")]
    public int              depth;
    public int              maxWidth;
    public int              randomSeed;
    public float            complexity;
    public bool             useSeed;
    public float            minWidthFraction;

    [Header("Traversal")]
    public EncounterInfo    currentEncounter;
    public List<int>        completedIndices = new List<int>(); // For regeneration!

    [Header("Fog Of War")]
    public GameEvent        FogUpdated;
    public int              layersRevealed  = 0;
    public int              lookAhead       = 2;

    public void InitializeStates()
    {
        // Traversal initial state
        if (randomSeed == 0) randomSeed = 1;
        completedIndices.Clear();
        completedIndices.Add(0);

        // Fog of war initial state
        layersRevealed = 1;
    }
    public void AddRevealed(int n)
    {
        layersRevealed += n;
        for (int i =  0; i < n; i++)
        {
            FogUpdated.Raise();            
        }
    }

    [Header("Visualization")]
    public int              layerDistance = 4;
    public int              encounterSep = 4;

}
