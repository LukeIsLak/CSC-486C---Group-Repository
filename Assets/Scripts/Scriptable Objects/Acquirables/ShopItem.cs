using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/ShopItem")]
public class ShopItem : ScriptableObject
{
    [Header("Data")]
    // Player gold container

    [Header("Item Info")]
    public int price = 0;
    public Acquirable acquirable;

    // Try to purchase the item. Return true and do acquire if the player is rich enough.
    public bool TryPurchase()
    {
        if (!acquirable) return false;
        /*
        if player gold < price return false

        player gold -= price
        acquirable.acquire()
        return true
        */
        return true;
    }
}
