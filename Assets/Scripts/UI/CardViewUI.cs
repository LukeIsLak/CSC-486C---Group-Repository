using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CardViewUI : MonoBehaviour
{
    public Cards card;
    public TextMeshProUGUI cardName;
    public Image cardImage;
    public Button button;
    public void Init(Cards cardData, bool isGrayOut = false, System.Action onBuffercardClick = null, bool buttonDisable = false)
    {
        card = cardData;    
        cardImage.sprite = card.image;
        if(isGrayOut) cardImage.color = new Color32(176, 176, 176, 255);
        cardName.text = cardData.name;
        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = !buttonDisable;
            if(onBuffercardClick != null)
            {
                button.onClick.AddListener(() => onBuffercardClick?.Invoke());
            }
        }
    }
}
