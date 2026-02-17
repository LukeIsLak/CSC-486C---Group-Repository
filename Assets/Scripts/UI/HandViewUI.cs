using System.Collections.Generic;
using UnityEngine;

public class HandViewUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private RectTransform handLocation;

    [Header("Card Fanning Visual Settings")]
    [SerializeField] private float totalFanAngle = 30f;
    [SerializeField] private float radius = 360f;  // bigger => flatter curve
    [SerializeField] private float selectedCardLift = 30f;
    [SerializeField] private float selectedScale = 1f;

    private List<CardViewUI> cards = new();
    private DeckSystems deckSystems;
    private void OnEnable()
    {
        Hook(deckSystems);
        UpdateCardPosition();
    }

    private void OnDisable()
    {
        Unhook(deckSystems);
    }

    public void BindDeckSystem(DeckSystems d)
    {
        if (deckSystems == d) return;
        // unsubsribe from the old event
        Unhook(deckSystems);
        deckSystems = d;
        Hook(deckSystems);
        RefreshHand();
    }
    
    // helper function for hooking the the function to event
    private void Hook(DeckSystems d)
    {
        if (d != null)
        {
            deckSystems.OnHandSelectionChanged += UpdateCardPosition;
            deckSystems.OnHandContentsChanged += RefreshHand;
        }
    }
    private void Unhook(DeckSystems d)
    {
        if (d != null)
        {
            deckSystems.OnHandSelectionChanged -= UpdateCardPosition;
            deckSystems.OnHandContentsChanged -= RefreshHand;
        }
    }
    // When hand contents change, rebuild the UI list to match the data.
    private void RefreshHand()
    {
        // Destroy all existing UI card gameObjects.
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();


        // Create the UI for each card instance in hand 
        foreach(var instance in deckSystems.hand)
        {

            CardViewUI card = Instantiate(cardViewPrefab, handLocation);
            card.Init(instance.cardData);
            cards.Add(card);
        }

        // update the card fanning after rebuilt the UI
        UpdateCardPosition();
    }

    private void UpdateCardPosition()
    {
        int cardCount = cards.Count;

        if (cardCount == 0) return;

        float startingCardAngle = -totalFanAngle / 2f; // starting card location Example: if card fan angle 30 starting will be -15 

        float angleStep; // how far apart each card is 
        

        // step between card. if there is 1 card, step will be 0 so that it stay in the center
        if (cardCount == 1)
        {
            angleStep = 0;
        }
        else
        {
            angleStep = totalFanAngle / (cardCount - 1);
        }

        RectTransform selectedCardTransform = null;

        for (int i = 0; i < cardCount; i++)
        {
            // Calculate the angle of this card
            float angleDeg = startingCardAngle + (angleStep * i);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // Get the card position on the circle arc 
            float x = radius * Mathf.Sin(angleRad);
            float y = radius * Mathf.Cos(angleRad) - radius;

            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();
            // set order of the card
            cardTransform.SetSiblingIndex(i);
            cardTransform.anchoredPosition = new Vector2(x, y);
            cardTransform.localRotation = Quaternion.Euler(0, 0, -angleDeg);

            if (i == deckSystems.currentHandIndex)
            {
                cardTransform.anchoredPosition += Vector2.up * selectedCardLift;
                cardTransform.localRotation = Quaternion.identity;
                selectedCardTransform = cardTransform;
            }
        }
        if (selectedCardTransform != null) selectedCardTransform.SetAsLastSibling();
    }
}
