using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Encounters/EncounterWeightList")]
public class EncounterWeightList : ScriptableObject
{
    public List<EncounterWeight> encounterWeights;
}
