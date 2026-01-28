using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// idea of this system being its created at the begining of the game and stays present through-out all scenes
public class DeckSystems : MonoBehaviour
{
    // holds GameObject reference/information of what cards are currently in the players hand
    // will be updated by drawcard()
    public GameObject handSlot1;
    public GameObject handSlot2;
    public GameObject handSlot3;
    public GameObject handSlot4;

    // The queue functions as the deck, with dequeue being equivelent to drawing a card, enqueue would be the same as putting a card back in to the deck at the bottom
    public Queue<GameObject> deck = new Queue<GameObject>();
    public int deckSize;
    public int currentDeckSize;
    public int maxDeckSize = 20; // to be updated once max size is decided

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// adds the given card to the back of the deck, 
    /// ASSUMPTION: when a card is colected the check for deck size happens, this function is for adding a used card back in to the deck
    /// </summary>
    /// <param name="card"></param>
    public void addCardToDeck(GameObject card)
    {
        // add card to deck
        deck.Enqueue(card);
        currentDeckSize++;
    }

    /// <summary>
    /// draws the next card in the deck queue, will assign that card to the hand slot it will ocupy and will return null if the handslot number give is invalid
    /// </summary>
    /// <param name="handslot"></param>
    /// <returns> null if handslot is invalid (0<handslot<5) or the card draw (GameObject) </returns>
    public GameObject drawCard(int handslot)
    {
        // remove first card from deck
        GameObject card = deck.Dequeue();

        // determine which handslot the card should take up
        switch (handslot)
        {
            case 1:
                handSlot1 = card;
                break;
            case 2:
                handSlot2 = card;
                break;
            case 3:
                handSlot3 = card;
                break;
            case 4:
                handSlot4 = card;
                break;
            default: // if invalid log the error and return null
                Debug.Log("error wrong value used on drawCard, value should be between 1-4");
                return null;
        }

        currentDeckSize--;
        // return the draw card for use
        return card;
    }

    public void shuffleExcHand() { 
        GameObject[] sortingArray = new GameObject[currentDeckSize];
        deck.CopyTo(sortingArray, 0);

        //Random rnd = new Random();
        while (currentDeckSize != 0) {
            //rnd.Next(currentDeckSize);
            //....
            
        }
    }
    //to add shuffle (not including hand), shuffle (including hand) load deck, tests, 
}
