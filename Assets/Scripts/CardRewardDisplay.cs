using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardRewardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro damageText;
    public void Init(string cardDescription, string cardName, string cardDamage)
    {
        descriptionText.text = cardDescription;
        nameText.text = cardName;
        damageText.text = "10";
    }
}
