using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Deck")]
public class Deck : ScriptableObject
{
    public List<CardInstance> contents;

    public int maxSize;
    public bool useMaxSize;

    public bool TransferCard(CardInstance card, Deck deck)
    {
        if (card.deck != this) return false; // Not mine to transfer
        if (deck.AddCard(card))
        {
            contents.Remove(card);
            return true;
        }
        return false;
        
    }

    public bool AddCard(CardInstance card)
    {
        if (card.deck == this) return false; // Already belongs
        if (useMaxSize && contents.Count == maxSize) return false; // Can't fit

        // Add to self and update cardinstance reference
        contents.Add(card);
        card.deck = this;
        return true;
    }

    public bool RemoveCard(CardInstance card)
    {
        return contents.Remove(card);
    }
    
}
