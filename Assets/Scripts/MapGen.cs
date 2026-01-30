using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGen : MonoBehaviour
{
    [SerializeField] private int maxDepth = 5;
    [SerializeField] private int maxWidth = 3;

    private List<List<MapNode>> layers;         // List whose entries are lists of the nodes at each layer
    private List<MapNode> currentLayer;         // Current layer being operated on


    public GameObject mapNodePrefab;            // Prefab for mapnodes

    /*********************
     Main Functionality
    *********************/

    // Initialize data structures, variables, random seed etc.
    void Initialize()
    {
        /*
        Data structures, variables, seeds etc.
        */

    }

    // Perform a compelete round of generation  
    void DoGeneration()
    {
        /*
        For each node in each layer
        Make generation choice
        Split? No -> Continue? No -> Merge? No -> Continue.

        Iterate again to create next layer
        Repeat
        */
    }

    /*********************
     Generation Helpers
    *********************/

    private void MakeGenerationChoice(MapNode node)
    {
        return;
    }
}
