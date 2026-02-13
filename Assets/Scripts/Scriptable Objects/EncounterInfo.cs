using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Encounters/EncounterInfo")]
public class EncounterInfo : ScriptableObject
{
    public int IND_ACCESSIBLE   = 0;
    public int IND_INACCESSIBLE = 1;
    public int IND_COMPLETED    = 2;
    public int IND_HOVERED      = 3;

    public List<Sprite> stateSprites;
    public List<EncounterWeight> encounterWeights;
    public SceneField scene;
}
