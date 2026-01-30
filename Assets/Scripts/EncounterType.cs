using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterType : ScriptableObject
{
    public string encounterName;
    private float branchInProb;         // Probability of branching in
    private float branchOutProb;        // Probability of branching out

    //TODO LK: Add more variables here when they are needed, this should be used to create the node
}
