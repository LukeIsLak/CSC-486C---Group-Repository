using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterDoor : MonoBehaviour
{   

    [Header("References")]
    public LayoutData layoutData;
    public GameEvent ExitToLayoutEvent;
    public string playerTag;

    [Header("Information")]
    public EncounterInfo encounter;
    public int index;
    public bool canUse;
    [Header("Child References")]
    public SpriteRenderer encounterIconSR;


    public void Initialize(EncounterInfo enc, int ind, LayoutData ld)
    {
        encounter = enc;
        index = ind;
        encounterIconSR.sprite = encounter.stateSprites[encounter.IND_ACCESSIBLE];
        layoutData = ld;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag) && canUse)
        DoDoorEntered();
    }

    void DoDoorEntered()
    {
        layoutData.completedIndices.Add(index);
        layoutData.currentEncounter = encounter;
        ExitToLayoutEvent.Raise();
        canUse = false;
    }

    // Call when another door exits so we don't have several triggers
    public void StopUsage()
    {
        canUse = false;
    }
}
