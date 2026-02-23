using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationChoice2
{
    None        = 0b0000,
    Split       = 0b0001,
    Forward     = 0b0010,
    MergeLeft   = 0b0100,
    MergeRight  = 0b1000,
    MergeBoth   = MergeLeft | MergeRight
}

public enum EncounterType2
{
    None        = 0b0000,
    Enemies     = 0b0001,
    Boss        = 0b0010,
    Shop        = 0b0100,
    Treasure    = 0b1000,
}

public class MapNode2 : MonoBehaviour
{
    /*********************
     Node Information
    *********************/

    // public MapBranch branch;                    // What branch does this belong to 
    public GenerationChoice2 choice;             // Choice made for generation
    private int numChildren;                    // Current number of children
    public List<MapNode2> outNodes;              // Nodes this one connects into
    // public EncounterType encounter;             // The encounter of the room
    public float branchInProbability = 1f;    // To be attached to the branch later. Probability of merging into this node from another.


    // Awake is called on initialization
    void Awake()
    {
        // branch      = null;
        choice      = GenerationChoice2.None;
        numChildren = 0;
        outNodes    = new List<MapNode2>();
        // encounter   = EncounterType.None;
    }

    /*********************
     Public Methods
    *********************/

    // Setters
    // public void SetEncounter(EncounterType e)   { encounter = e; }

    // public void SetBranch(MapBranch b)          { branch = b; }


    // Child insertion
    public void AddChildLeft(MapNode2 child)     { outNodes.Insert(0, child); numChildren++; }

    public void AddChildRight(MapNode2 child)    { outNodes.Add(child); numChildren++; }


    // Getters
    // public EncounterType GetEncounter()         { return encounter; }

    // public MapBranch GetBranch()                { return branch; }

    public List<MapNode2> GetOutNodes()          { return outNodes; }

    public MapNode2 GetLeftmostChild()           { return numChildren == 0 ? null : outNodes[0]; }

    public MapNode2 GetRightmostChild()          { return numChildren == 0 ? null : outNodes[numChildren - 1]; }
}
