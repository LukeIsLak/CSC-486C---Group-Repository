using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    [Header("Data")]
    public MerchantData merchantData;
    public ShopItemList shopItemDB;
    public RandomContext randomContext;

    [Header("Events")]
    public GameEvent MerchantPopulated;

    [Header("Parameters")]
    public int shopItemCount = 5;

    void Start()
    {
        // For now, the merchant shows all items
        merchantData.wares.Clear();
        for (int i = 0; i < shopItemCount; i++)
        {
            merchantData.wares.Add(ShopItemList.MakeWeightedChoice(shopItemDB.items, randomContext));
            Debug.Log("Populating");
        }
        MerchantPopulated.Raise();
    }
}
