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
    public GameEvent EnterLayout;
    public GameEvent MerchantPopulated;

    [Header("Parameters")]
    public int shopItemCount = 5;

    private bool hasExited = false;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (hasExited) return;
            hasExited = true;
            EnterLayout.Raise();
        }
    }
}
