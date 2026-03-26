using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragHandler : MonoBehaviour
{
    // PURPOSE

    // Store most recently hovered over "deck" (player deck, buffer, or sideboard)
    // Set currently dragged card's parent to self
    // On end, check card's origin deck to determine if updates need to happen
    // As in ,f i
    // Start is called before the first frame update
    
    public Transform oldParent;
    public static DragHandler instance;
    public Deck hoveredDeck;
    public Transform hoveredTransform;

    public NodeTraversalUI nodeTraversalUI;

    void Awake()
    {
        if (instance && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
        }
    }
    public void HandleDragStart(GameObject card)
    {
        oldParent = card.transform.parent;
        card.transform.SetParent(transform, true);
    }

    public bool HandleDragEnd(GameObject card, CardInstance cardInstance)
    {
        // Check if a deck is hovered and then check if we can TRANSFER
        if (hoveredDeck && cardInstance.deck.TransferCard(cardInstance, hoveredDeck))
        {
            nodeTraversalUI.RefreshUI();
            return true;

        }
        // Couldn't add to the deck, return false and reset parent
        card.transform.SetParent(oldParent, true);
        return false;
    }

    // 
    public void SetHoveredDeck(Deck deck, Transform t)
    {
        hoveredDeck = deck;
        hoveredTransform = t;
    }

    public void UnsetHoveredDeck(Deck deck)
    {
        if (hoveredDeck == deck)
        {
            hoveredDeck = null;
            hoveredTransform = null;
        }
    }
}
