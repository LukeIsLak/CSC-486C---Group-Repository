using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBranchType : ScriptableObject
{
    public string BranchTypeName;
    public List<EncounterType> possibleEncounters;
    public List<double> encounterProbabilities;
    public int minDepth;
    public int maxDepth;

    public bool repeatable = false;
}
