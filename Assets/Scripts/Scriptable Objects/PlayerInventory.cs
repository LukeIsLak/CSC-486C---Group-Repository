using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    
    public List<Cards> startingHand;    // The hand the player begins with
    public int currency;
    public List<CardInstance> playerdeck = new();
    public List<CardInstance> buffer = new();
    public int nextUid = 0;

    public int amountRemovalTokens = 0;

    public void Initialize()
    {
        currency = 0;
        playerdeck.Clear();
        buffer.Clear();
        nextUid = 0;
        amountRemovalTokens = 0;

        foreach (Cards card in startingHand)
        {
            playerdeck.Add(new CardInstance(card, nextUid++));
        }

    }

    // call this in game manager.. or doooont since we initialize with a fixed hand!
    public void InitializeRandomDeck(CardsDatabase cardsDB, int deckSize)
    {
        // here so the deck doesnt explode in size
        playerdeck.Clear();
        nextUid = 0;
        //Debug.Log("im here");
        for (int i = 0; i < deckSize; i++)
        {
            Cards pick = cardsDB.allCards[Random.Range(0, cardsDB.allCards.Count)];
            playerdeck.Add(new CardInstance(pick, nextUid++));
        }

        
    }

    /// <summary>
    /// used to add currency to a players inventory
    /// </summary>
    /// <param name="amount"></param>
    public void addCurrency(int amount) {
        currency += amount;
    }

    /// <summary>
    /// checks to see if a given amount can be taken out of a player currency if that is the case it will go ahead and do it
    /// </summary>
    /// <param name="amount"></param>
    /// <returns> returns a bool if a player can "pay" for the amount given, if true that amount will be taken out </returns>
    public bool subtractCurrency(int amount) {
        if (currency - amount >= 0) { 
            currency -= amount;
            return true;
        }
        return false;
    }

    public void AddToBuffer(CardInstance card){
        buffer.Add(card);
    }

    public void clearBuffer(){
        buffer.Clear();
    }

    public void bufferToDeck(CardInstance card){
        for (int i = 0; i < buffer.Count; i++){
            if (buffer[i] == card){
                buffer.RemoveAt(i);
                playerdeck.Add(card);
                return; // end process as card was found
            }
        }
        
    }

    // returns true if the player has a token and the card is removed from their deck, returns false if the play does not have a token or the card cant be found
    public void removeCardFromPlayersDeck(CardInstance card){
        for (int i = 0; i < playerdeck.Count; i++){
            if (playerdeck[i] == card){
                playerdeck.RemoveAt(i);
            }
        }
    }

    public void AddRemovalToken(){
        amountRemovalTokens++;
    }

    public void refreshCards(){
        for (int i = 0; i < playerdeck.Count; i++){
            playerdeck[i].useable = true;
        }
    }
}
