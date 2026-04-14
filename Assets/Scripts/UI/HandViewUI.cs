using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandViewUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private Dictionary<CardViewUI, Vector2> cardTargetPositions = new();
    [SerializeField] private RectTransform handLocation;

    [Header("Card Fanning Visual Settings")]
    [SerializeField] private float totalFanAngle = 30f;
    [SerializeField] private float radius = 360f;  // bigger => flatter curve
    [SerializeField] private float selectedCardLift = 30f;
    [SerializeField] private float selectedScale = 1f;

    private List<CardViewUI> cards = new();
    private DeckSystems deckSystems;

    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnEnable()
    {
        Hook(deckSystems);
        UpdateCardPosition();
    }

    private void OnDisable()
    {
        Unhook(deckSystems);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DeleteAllCardUI();
    }

    private void DeleteAllCardUI()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            cardTargetPositions.Remove(cards[i]);
            Destroy(cards[i].gameObject);
        }
        cards.Clear();
    }

    public void BindDeckSystem(DeckSystems d)
    {
        if (deckSystems == d) return;
        // unsubsribe from the old event
        Unhook(deckSystems);
        deckSystems = d;
        Hook(deckSystems);
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

    private void Update()
    {
        float lerpSpeed = 10f;
        foreach (var card in cards)
        {
            if (card == null) continue;
            RectTransform cardTransform = card.GetComponent<RectTransform>();
            if (cardTargetPositions.TryGetValue(card, out Vector2 targetPos))
            {
                cardTransform.anchoredPosition = Vector2.Lerp(
                    cardTransform.anchoredPosition,
                    targetPos,
                    Time.deltaTime * lerpSpeed
                );
            }
        }
    }

    // When hand contents change, rebuild the UI list to match the data.
    private void RefreshHand()
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            var cardView = cards[i];
            if (cardView.cardInstance == null || !deckSystems.hand.Exists(card => card != null && card.uid == cardView.cardInstance.uid))
            {
                cardTargetPositions.Remove(cardView);
                Destroy(cardView.gameObject);
                cards.RemoveAt(i);
            }
        }

        foreach (var instance in deckSystems.hand)
        {
            bool alreadyHasUI = cards.Exists(cv => cv.cardInstance.uid == instance.uid);
            if (!alreadyHasUI)
            {
                CardViewUI card = Instantiate(cardViewPrefab, handLocation);
                card.Init(instance, drawnCard: true);
                cards.Add(card);
            }
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

        for (int i = cardCount - 1; i >= 0; i--)
        {
            int displayIndex = cardCount - 1 - i;
            // Calculate the angle of this card
            float angleDeg = startingCardAngle + (angleStep * displayIndex);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // Get the card position on the circle arc 
            float x = radius * Mathf.Sin(angleRad);
            float y = radius * Mathf.Cos(angleRad) - radius;

            RectTransform cardTransform = cards[i].GetComponent<RectTransform>();
            cardTransform.SetSiblingIndex(displayIndex);

            // Store the target position
            cardTargetPositions[cards[i]] = new Vector2(x, y);

            cardTransform.localRotation = Quaternion.Euler(0, 0, -angleDeg);

            if (i == deckSystems.currentHandIndex)
            {
                cardTargetPositions[cards[i]] += Vector2.up * selectedCardLift;
                cardTransform.localRotation = Quaternion.identity;
                selectedCardTransform = cardTransform;
            }
        }
        if (selectedCardTransform != null) selectedCardTransform.SetAsLastSibling();
    }
}
