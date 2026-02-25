using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private MerchantData merchantData;
    [SerializeField] private MerchantItemUI merchantItemPrefab;
    [SerializeField] private RectTransform contentLocation; //place for instantiate prefabs


    private List<MerchantItemUI> merchantItemUIs = new();
    
    private HashSet<MerchantItemUI> purchasedSlot = new(); // use set for tracking purchased items


    private void OnEnable()
    {
        RebuildUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void RebuildUI()
    {
        ClearUI();
        if (merchantData == null) return;
        if (merchantData.wares == null)
        {
            Debug.LogError("merchant ware null");
        }

        foreach (var shopItem in merchantData.wares)
        {
            if (shopItem == null) continue;

            MerchantItemUI item = Instantiate(merchantItemPrefab, contentLocation);
            item.Init(shopItem,TryBuying);
            merchantItemUIs.Add(item);
        }
    }

    private void TryBuying(ShopItem item, MerchantItemUI itemUI)
    {
        if (purchasedSlot.Contains(itemUI))
        {
            itemUI.SetPurchased(true);
            return;
        }

        bool success = item.TryPurchase();

        if (success)
        {
            purchasedSlot.Add(itemUI);
            itemUI.SetPurchased(true);
        }
        else
        {
            itemUI.CantAfford();
        }
    }
    private void ClearUI()
    {
        foreach(var ui in merchantItemUIs)
        {
            if(ui != null) Destroy(ui.gameObject);
        }
        merchantItemUIs.Clear();
        purchasedSlot.Clear();
    }
}
