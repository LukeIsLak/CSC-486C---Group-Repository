using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardViewUI : MonoBehaviour
{
    public Cards card;
    public TextMeshProUGUI cardName;
    public void Init(Cards cardData)
    {
        card = cardData;
        cardName.text = cardData.name;
    }
}
