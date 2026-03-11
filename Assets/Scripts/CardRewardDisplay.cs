using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardRewardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro descriptionText;
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public void Init(Cards card)
    {
        descriptionText.text = card.descrption;
        nameText.text = card.name;
        damageText.text = card.effectValue.ToString();
        spriteRenderer.sprite = card.image;
    }
}
