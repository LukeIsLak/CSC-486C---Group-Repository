using System.Collections.Generic;
using UnityEngine;

public class HandViewUI : MonoBehaviour
{
    [SerializeField] private DeckSystems deckSystems;


    [Header("UI References")]
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private RectTransform handLocation;

    [Header("Card Fanning Visual Settings")]
    [SerializeField] private float totalFanAngle = 30f;
    [SerializeField] private float radius = 360f;  // bigger => flatter curve
    [SerializeField] private float selectedCardLift = 30f;
    [SerializeField] private float selectedScale = 1f;


    private readonly List<CardViewUI> cards = new();
    private void OnEnable()
    {
        deckSystems.OnHandSelectionChanged += UpdateCardPosition;
        deckSystems.OnHandContentsChanged += RefreshHand;
        UpdateCardPosition();
    }

    private void OnDisable()
    {
        deckSystems.OnHandSelectionChanged -= UpdateCardPosition;
        deckSystems.OnHandContentsChanged -= RefreshHand;
    }

    private void RefreshHand()
    {
        foreach(var card in cards)
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

        UpdateCardPosition();
    }

    private void UpdateCardPosition()
    {
        int cardCount = cards.Count;

        if (cardCount == 0) return;

        float startingCardAngle = -totalFanAngle / 2f; // starting card location Example: if card fan angle 30 starting will be -15 
        float angleStep; // how far apart each card is 
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
            float angleDeg = startingCardAngle + (angleStep * i);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = radius * Mathf.Sin(angleRad);
            float y = radius * Mathf.Cos(angleRad) - radius;

            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();
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
