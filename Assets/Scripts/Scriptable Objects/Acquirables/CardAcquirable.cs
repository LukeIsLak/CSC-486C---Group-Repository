using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/CardAcquirable")]

public class CardAcquirable : Acquirable
{
    [Header("Data")]
    // Player inventory
    public PlayerInventory playerInventory;
    public Cards card; // Or should it be a cardinstance?

    public override void Acquire()
    {
        playerInventory.AddToBuffer(new CardInstance(card)); // Make a card instance here!
        // Assign UID when?
    }
}
