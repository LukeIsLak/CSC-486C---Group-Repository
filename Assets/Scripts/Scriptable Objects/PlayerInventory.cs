using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public List<CardInstance> playerDeck = new();
    public List<Cards> allCards;

    // Start is called before the first frame update
    void Awake()
    {
        playerDeck.Clear();

        for (int i = 0; i < 30; i++) 
        {
            Cards pick = allCards[Random.Range(0, allCards.Count)];
            playerDeck.Add(new CardInstance(pick));
        }
    }
}
