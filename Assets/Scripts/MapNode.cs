using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenerationChoice
{
    None        = 0b0000,
    Split       = 0b0001,
    Continue    = 0b0010,
    MergeLeft   = 0b0100,
    MergeRight  = 0b1000,
    MergeBoth   = MergeLeft | MergeRight
}

public enum EncounterType
{
    None        = 0b0000,
    Enemies     = 0b0001,
    Boss        = 0b0010,
    Shop        = 0b0100,
    Treasure    = 0b1000,
}

public class MapNode : MonoBehaviour
{
    /*********************
     Node Information
    *********************/

    private MapBranch branch;                   // What branch does this belong to 
    public GenerationChoice choice;             // Choice made for generation
    private int numChildren;                    // Current number of children
    private List<MapNode> inNodes;              // Nodes connecting into this one
    private List<MapNode> outNodes;             // Nodes this one connects into
    public EncounterType encounter;             // The encounter of the room


    // Awake is called on initialization
    void Awake()
    {
        branch      = null;
        choice      = GenerationChoice.None;
        numChildren = 0;
        inNodes     = new List<MapNode>();
        outNodes    = new List<MapNode>();
        encounter   = EncounterType.None;
    }

    /*********************
     Public Methods
    *********************/

    // Setters
    public void SetEncounter(EncounterType e)   { encounter = e; }

    public void SetBranch(MapBranch b)          { branch = b; }


    // Child insertion
    public void AddChildLeft(MapNode child)     { outNodes.Insert(0, child); }

    public void AddChildRight(MapNode child)    { outNodes.Add(child); }


    // Getters
    public EncounterType GetEncounter()         { return encounter; }

    public MapBranch GetBranch()                { return branch; }

    public List<MapNode> GetOutNodes()          { return outNodes; }

    public List<MapNode> GetInNodes()           { return inNodes;  }  

    public MapNode GetLeftmostChild()           { return numChildren == 0 ? null : outNodes[0]; }

    public MapNode GetRightmostChild()          { return numChildren == 0 ? null : outNodes[numChildren - 1]; }

}
