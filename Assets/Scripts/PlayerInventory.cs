using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // note will need to update gameobject to card
    public GameObject[] playerDeck = new GameObject[15];
    public int currency;
    public int cardsInDeck;
    
    // Start is called before the first frame update
    void Start()
    {
        if (true) { // change to check if it should pull information from save file
            currency = 0;
        }
        // deck = {fill out with cards for alpha}
        // cardsInDeck = ?;
        // after alpha add in check to see if there is a saved deck from previous play or if a new character has been made and should use a starter deck
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// with two given indexs swap those cards in the players deck (this has no effect on how cards will be shuffled, more for player satisfaction)
    /// </summary>
    /// <param name="posA"></param>
    /// <param name="PosB"></param>
    public void swapCardPosition(int posA, int PosB) {
        GameObject temp = playerDeck[posA];
        playerDeck[posA] = playerDeck[PosB];
        playerDeck[PosB] = temp;
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
        if (currency - amount > 0) { 
            currency -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// this function is called if a player has confirmed they want to add a card to their deck, if there deck is full it will call a function to make them discard a card, 
    /// otherwise will add this card to the back of their deck
    /// </summary>
    /// <param name="card"></param>
    /// <returns> returns true if player added card to deck, if card is not added return false </returns>
    public bool addCardToPlayerDeck(GameObject card) {
        if (cardsInDeck == 15) {
            //prompt player to discard a card
            //if player discards a card, return true and replace card
            //else return false as card was not added

            //temporary
            return false; //temporary
            //temporary
        }
        else { 
            playerDeck[cardsInDeck-1] = card;
            return true;
        }
    }
}
