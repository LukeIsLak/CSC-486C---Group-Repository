using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MerchantItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image icon;
    //[SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    public void Init(ShopItem shopitem, System.Action<ShopItem, MerchantItemUI> onBuyClicked)
    {
        description.text = shopitem.acquirable.GetName();
        //nameText.text = shopitem.acquirable.itemName;
        icon.sprite = shopitem.acquirable.icon;
        priceText.text = shopitem.price.ToString();
        TooltipTrigger tooltip = icon.GetComponent<TooltipTrigger>();
        if (tooltip != null) 
        {
            CardAcquirable card = (CardAcquirable)shopitem.acquirable;
            tooltip.SetContent(card.GetName(), card.GetDescription());
        }
        if (buyButton != null) 
        {
            buyButton.onClick.RemoveAllListeners();
            // subscribe the trybuy function in merchantUi to the button of the item
            buyButton.onClick.AddListener (() => onBuyClicked?.Invoke(shopitem, this));
        }
    }

    public void SetPurchased(bool bpurchased)
    {
        if (buyButton != null) buyButton.interactable = !bpurchased;
    }

    public void CantAfford()
    {
        Debug.Log("you are too poor");
    }
}
