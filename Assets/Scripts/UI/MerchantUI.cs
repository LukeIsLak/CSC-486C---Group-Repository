using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MerchantUI : MonoBehaviour
{
    [SerializeField] private MerchantData merchantData;
    [SerializeField] private MerchantItemUI merchantItemPrefab;
    [SerializeField] private RectTransform contentLocation; //place for instantiate prefabs
    [SerializeField] private TextMeshProUGUI goldNum;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private ShopItem token;
    [SerializeField] private TextMeshProUGUI tokenNum;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject inRoomShopKeeper;
    
    private List<MerchantItemUI> merchantItemUIs = new();
    
    private HashSet<MerchantItemUI> purchasedSlot = new(); // use set for tracking purchased items

    
    private void OnEnable()
    {
        RebuildUI();
        Cursor.lockState    = CursorLockMode.None;
        Cursor.visible      = true;
        inRoomShopKeeper.SetActive(false);
    }

    private void OnDisable()
    {
        Cursor.lockState    = CursorLockMode.Locked;
        Cursor.visible      = false;
        inRoomShopKeeper.SetActive(true);
    }
    

    public void RebuildUI()
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
        dialogueText.text = merchantData.GetOpener();
        UpdateGoldDisplay();
        UpdateTokenDisplay();
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
            UpdateGoldDisplay();
        }
        else
        {
            itemUI.CantAfford();
        }
        dialogueText.text = merchantData.GetResponse(success);
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

    private void UpdateGoldDisplay()
    {
        goldNum.text = playerInventory.currency.ToString();
    }
    private void UpdateTokenDisplay()
    {
       tokenNum.text = playerInventory.amountRemovalTokens.ToString();
    }
    public void BuyToken()
    {
        bool success = token.TryPurchase();
        dialogueText.text = merchantData.GetResponse(success);
        UpdateTokenDisplay();
        UpdateGoldDisplay();
    }
        
}
