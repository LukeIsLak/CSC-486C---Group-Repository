using System.Collections.Generic;
using UnityEngine;

public class HandViewUI : MonoBehaviour
{
    [SerializeField] private DeckSystems deckSystems;

    [Header("Card Fanning Visual Settings")]
    [SerializeField] private float totalFanAngle = 30f;
    [SerializeField] private float radius = 350f;  // bigger => flatter curve
    [SerializeField] private float handHeight = -80f;
    [SerializeField] private float selectedCardLift = 30f;

    private void OnEnable()
    {
        deckSystems.OnHandChanged += UpdateCardPosition;

        UpdateCardPosition();
    }

    private void OnDisable()
    {
        deckSystems.OnHandChanged -= UpdateCardPosition;
    }
    private void UpdateCardPosition()
    {
        List<GameObject> cards = deckSystems.hand;
        int cardCount = deckSystems.hand.Count;
        
        if (cardCount == 0) return;

        float startingCardAngle = -totalFanAngle / 2f; // starting card location Example: if card fan angle 30 starting will be -15 
        float angleStep; // how far apart each card is 
        if(cardCount == 1)
        {
            angleStep = 0;
        }
        else
        {
            angleStep = totalFanAngle / (cardCount-1);
        }

        for(int i = 0;i < cardCount; i++)
        {
            float angleDeg = startingCardAngle + (angleStep*i);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = radius * Mathf.Sin(angleRad);
            float y = radius * Mathf.Cos(angleRad) - radius;
            y += handHeight;

            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();

            cardTransform.anchoredPosition = new Vector2(x, y);
            cardTransform.localRotation = Quaternion.Euler(0, 0, -angleDeg);
            if (i == deckSystems.currentHandIndex)
            {
                cardTransform.anchoredPosition += Vector2.up * selectedCardLift;
            }
        }
    }
}
