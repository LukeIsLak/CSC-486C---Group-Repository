using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseDeck : MonoBehaviour
{
    public bool chosen;
    public PlayerInventory playerInventory;
    public List<Cards> startingDeck;

    public RectTransform contentLocation;
    public CardViewUI cardViewUIPrefab;

    void Start()
    {
        chosen = false;
        foreach (Cards card in startingDeck)
        {
            CardViewUI cvUI = Instantiate(cardViewUIPrefab, contentLocation);
            cvUI.Init(card);
            TooltipTrigger tooltipTrigger = cvUI.GetComponent<TooltipTrigger>();
            if(tooltipTrigger != null)
            {
                tooltipTrigger.SetContent(card.name, card.descrption);
            }
        }
    }

    public void SetStartingDeck()
    {
        if (chosen) return;
        playerInventory.startingHand = startingDeck;
    }

    public void StopChoice()
    {
        chosen = true;
    }
}
