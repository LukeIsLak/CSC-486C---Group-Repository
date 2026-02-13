using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Encounters/EncounterWeight")]
public class EncounterWeight : ScriptableObject
{
    public EncounterInfo encounter;
    public float weight;
}
