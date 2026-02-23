using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Acquirable/ShopItem")]
public class ShopItem : ScriptableObject
{
    [Header("Data")]
    public PlayerInventory playerInventory;

    [Header("Item Info")]
    public int price = 0;
    public Acquirable acquirable;

    // Try to purchase the item. Return true and do acquire if the player is rich enough.
    public bool TryPurchase()
    {
        if (!acquirable) return false;
        if (!playerInventory.subtractCurrency(price)) return false; // Can't afford, no substraction
        acquirable.Acquire();
        return true;
    }
}
