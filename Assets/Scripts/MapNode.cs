using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    /*********************
     DATA STRUCTURES
    *********************/

    private string encounterType;       // What type of encounter is this
    private MapBranch branch;           // What branch does this belong to 
    private float branchInProb;         // Probability of branching in
    private float branchOutProb;        // Probability of branching out
    private List<MapNode> inNodes;      // Nodes connecting into this one
    private List<MapNode> outNodes;     // Nodes this one connects into
    
    // Awake is called on initialization
    void Awake()
    {
        return;
    }

    /*********************
     GETTERS AND SETTERS
    *********************/
    // Getters
    public string 
    GetEncounterType() { return encounterType; }

    public MapBranch     
    GetBranch() { return branch; }

    public float 
    GetBranchInProb() { return branchInProb; }

    public float 
    GetBranchOutProb() { return branchOutProb; }

    public List<MapNode>
    GetOutNodes() { return outNodes; }

    public List<MapNode>
    GetInNodes()  { return inNodes;  }  
    
    // Setters
    public void
    SetEncounterType(string e) { encounterType = e; }

    public void
    SetBranch(MapBranch b) { branch = b; }

    public void 
    SetBranchInProb(float p) { branchInProb = p; }

    public void 
    SetBranchOutProb(float p) { branchOutProb = p; }

    // In/Out nodes lists will be accessed and modified by reference


}
