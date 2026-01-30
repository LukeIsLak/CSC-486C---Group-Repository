using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBranch : MonoBehaviour
{
    /*********************
     DATA STRUCTURES
    *********************/
    
    /* Note LK: I've set branchType as a ScriptableObject for now so that designers can control the rules,
        parameters, etc.. We may want to change this in the future.

       Furthermore, I've created a temp function called convertToBinary(). This does nothing for now, but
       this should save the branch / map to a binary file to save between save states.
    */
    private MapBranchType branchType;
    private List<MapNode> branchNodes;


    void Awake() {
        return;
    }

    /*********************
     GETTERS AND SETTERS
    *********************/
    // Getters
    public MapBranchType
    GetBranchType() {return branchType;}

    public List<MapNode>
    GetBranchNodes() {return branchNodes;}

    public void
    SetBranchType(MapBranchType t) {branchType = t;} 
    // Setters

    /*********************
     HELPER FUNCTIONS
    *********************/
}
