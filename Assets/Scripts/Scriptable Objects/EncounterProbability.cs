using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Encounters/EncounterProbability")]
public class EncounterProbability : ScriptableObject
{
    public EncounterInfo encounter;
    public float probability;
}
