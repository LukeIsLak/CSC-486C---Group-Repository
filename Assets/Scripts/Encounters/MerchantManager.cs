using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    [Header("Data")]
    public MerchantData merchantData;
    public ShopItemList shopItemDB;

    [Header("Events")]
    public GameEvent EnterLayout;
    public GameEvent MerchantPopulated;


    void Start()
    {
        // For now, the merchant shows all items
        merchantData.wares = shopItemDB.items;
        MerchantPopulated.Raise();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
