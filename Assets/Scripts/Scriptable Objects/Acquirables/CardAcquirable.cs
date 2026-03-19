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
        playerInventory.AddToBuffer(new CardInstance(card ,playerInventory.nextUid++)); // Make a card instance here!
        // Assign UID when?
    }

    public override string GetName()
    {
        return card.name;
    }

    public override string GetDescription()
    {
        return card.descrption;
    }
}
