using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class CardViewUI : MonoBehaviour
{
    public Cards card;
    public CardInstance cardInstance;
    public TextMeshProUGUI cardName;
    public Image cardImage;
    public TextMeshProUGUI cardDamage;
    public TextMeshProUGUI cardDescription;
    public Button button;

    public Animator anim;
    public AnimationClip drawEffect;
    public AnimationClip useEffect;

    public void Init(Cards cardData, bool isGrayOut = false, System.Action onCardClick = null, bool buttonDisable = false, bool drawnCard = false)
    {
        if (drawnCard) StartCoroutine(cardViewDelay(cardData, isGrayOut, onCardClick, buttonDisable));
        else {
            card = cardData;    
            cardImage.sprite = card.image;
            if(isGrayOut) cardImage.color = new Color32(176, 176, 176, 255);
            cardName.text = cardData.name;
            cardDamage.text = cardData.effectValue.ToString();
            cardDescription.text = cardData.descrption;
            cardInstance = null;

            if(button != null)
            {
                button.onClick.RemoveAllListeners();
                button.interactable = !buttonDisable;
                if(onCardClick != null)
                {
                    button.onClick.AddListener(() => onCardClick?.Invoke());
                }
            }
        }
    }

    public void Init(CardInstance cardInstance, bool isGrayOut = false, System.Action onCardClick = null, bool buttonDisable = false, bool drawnCard = false)
    {
        if (drawnCard) StartCoroutine(cardIViewDelay(cardInstance, isGrayOut, onCardClick, buttonDisable));
        else {
            card = cardInstance.cardData;    
            cardImage.sprite = card.image;
            if(isGrayOut) cardImage.color = new Color32(176, 176, 176, 255);
            cardName.text = cardInstance.cardData.name;
            cardDamage.text = cardInstance.cardData.effectValue.ToString();
            cardDescription.text = cardInstance.cardData.descrption;
            this.cardInstance = cardInstance;

            if(button != null)
            {
                button.onClick.RemoveAllListeners();
                button.interactable = !buttonDisable;
                if(onCardClick != null)
                {
                    button.onClick.AddListener(() => onCardClick?.Invoke());
                }
            }
        }
    }

    public IEnumerator cardViewDelay(Cards cardData, bool isGrayOut = false, System.Action onCardClick = null, bool buttonDisable = false) {
        anim.SetTrigger("getCard");
        yield return new WaitForSeconds(drawEffect.length);
        card = cardData;    
        cardImage.sprite = card.image;
        if(isGrayOut) cardImage.color = new Color32(176, 176, 176, 255);
        cardName.text = cardData.name;
        cardDamage.text = cardData.effectValue.ToString();
        cardDescription.text = cardData.descrption;
        cardInstance = null;

        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = !buttonDisable;
            if(onCardClick != null)
            {
                button.onClick.AddListener(() => onCardClick?.Invoke());
            }
        }
    }

    public IEnumerator cardIViewDelay (CardInstance cardInstance, bool isGrayOut = false, System.Action onCardClick = null, bool buttonDisable = false) {
        anim.SetTrigger("getCard");
        card = cardInstance.cardData;
        this.cardInstance = cardInstance;
        yield return new WaitForSeconds(drawEffect.length); 
        cardImage.sprite = card.image;
        if(isGrayOut) cardImage.color = new Color32(176, 176, 176, 255);
        cardName.text = cardInstance.cardData.name;
        cardDamage.text = cardInstance.cardData.effectValue.ToString();
        cardDescription.text = cardInstance.cardData.descrption;

        if(button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = !buttonDisable;
            if(onCardClick != null)
            {
                button.onClick.AddListener(() => onCardClick?.Invoke());
            }
        }
    }
}
