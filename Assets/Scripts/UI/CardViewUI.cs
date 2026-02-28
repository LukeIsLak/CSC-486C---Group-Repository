using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CardViewUI : MonoBehaviour
{
    public Cards card;
    public TextMeshProUGUI cardName;
    public void Init(Cards cardData)
    {
        card = cardData;    
        this.GetComponent<Image>().sprite = card.image;
        cardName.text = cardData.name;
    }
}
