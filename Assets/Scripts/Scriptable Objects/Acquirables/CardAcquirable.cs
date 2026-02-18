using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/CardAcquirable")]

public class CardAcquirable : Acquirable
{
    [Header("Data")]
    // Player inventory
    public Cards card; // Or should it be a cardinstance?
    public override void Acquire()
    {
        // add this card to player inventory buffer
    }
}
