using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/GoldAcquirable")]
public class GoldAcquirable : Acquirable
{
    public PlayerInventory playerInventory;
    public int amount;
    public override void Acquire()
    {
        playerInventory.addCurrency(amount);
    }
}
