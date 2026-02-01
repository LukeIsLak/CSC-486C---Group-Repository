using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// idea of this system being its created at the begining of the game and stays present through-out all scenes
public class DeckSystems : MonoBehaviour
{
    //stores a version of the deck to be used for card inspections and general deck viewing, will be the deck loaded in at the begining of encounters
    public List<GameObject> PlayerDeckCosmectic = new List<GameObject>();

    // holds GameObject reference/information of what cards are currently in the players hand
    // will be updated by drawcard()
    public GameObject[] hand = new GameObject[5];
    //let the system know how many cards are currently in a player hand
    public int currentHandSize = 0;
    
    // signifys the card the player currently has selected
    public int currentHandIndex = 0;

    //constant for accesing specifc hand slots
    const int HANDSLOT1INDEX = 0;
    const int HANDSLOT2INDEX = 1;
    const int HANDSLOT3INDEX = 2;
    const int HANDSLOT4INDEX = 3;
    const int HANDSLOT5INDEX = 4;

    // const to help define max and min hand size
    const int MAXHANDSIZE = 5;
    const int EMPTYHANDSIZE = 0;

    // The queue functions as the deck, with dequeue being equivelent to drawing a card, enqueue would be the same as putting a card back in to the deck at the bottom
    public Queue<GameObject> deck = new Queue<GameObject>();

    // used out of combat (total deck size)
    public int deckSize;

    // used in combat (cards left in deck during level, ex. doesnt not include hand)
    public int currentDeckSize;

    // the maximum amount of cards a player can have in a deck
    const int MAXDECKSIZE = 15;

    //store any card that has been used
    public List<GameObject> discard = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame, will check if the player hand is empty, if that is the case then fill back up to 5 if possible
    void Update()
    {
        // might want to put a delay on this
        if (currentHandSize == EMPTYHANDSIZE && currentDeckSize > 0) { // a check here to hopfully save some execution time by not trigering the loop
            for (int i = 0; i < MAXHANDSIZE; i++)
            {
                if (currentDeckSize > 0) { // in case a full hand isnt avalible
                    drawCard(i);
                }
            }
        }
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
    /// <returns> null if handslot is invalid (less than 0 or greater than 5) or the card draw (GameObject) </returns>
    public GameObject drawCard(int handslot)
    {
        // remove first card from deck
        GameObject card = deck.Dequeue();

        // check handslot is valid
        if (handslot < 0 || handslot > 4) { 
            Debug.Log("error wrong value used on drawCard, value should be between 0-4");
            return null;
        }

        hand[handslot] = card;

        currentDeckSize--;
        currentHandSize++;
        // return the draw card for use
        return card;
    }

    /// <summary>
    /// will suffle the player hand but will exclude any card assigned to the player hand, insperation: https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
    /// </summary>
    public void shuffleExcHand() {
        //temporary list to aid in suffeling (abbility to pull specific indexes) 
        List<GameObject> sortingList = new List<GameObject>();
        int listSize = currentDeckSize;
        currentDeckSize = 0;

        //empty the deck in to the sorting list
        for (int i = 0; i < listSize; i++) { 
            sortingList.Add(deck.Dequeue());
        }

        //randomly select a index and then put that card in to the deck
        int index = 0;
        while (listSize != 0) {
            index = Random.Range(0, listSize);
            addCardToDeck(sortingList[index]);
            sortingList.RemoveAt(index);
            listSize--;
        }
    }

    /// <summary>
    /// put the players hand in to the deck, then shuffle and redraw the player hand
    /// </summary>
    public void shuffleIncHand() {
        // add the cards in to the players deck
        addCardToDeck(hand[HANDSLOT1INDEX]);
        addCardToDeck(hand[HANDSLOT2INDEX]);
        addCardToDeck(hand[HANDSLOT3INDEX]);
        addCardToDeck(hand[HANDSLOT4INDEX]);
        addCardToDeck(hand[HANDSLOT5INDEX]);

        //shuffle the deck now it has the players hand in it
        shuffleExcHand();

        //redraw the hand
        drawCard(HANDSLOT1INDEX);
        drawCard(HANDSLOT2INDEX);
        drawCard(HANDSLOT3INDEX);
        drawCard(HANDSLOT4INDEX);
        drawCard(HANDSLOT5INDEX);
    }

    /// <summary>
    /// will take an array containing cards and add the deck in to the system filling out both the hand and deck
    /// </summary>
    /// <param name="passedDeck"></param>
    /// <param name="size"></param>
    /// <returns> will return true if sucsesful </returns>
    public bool loadDeck(List<GameObject> passedDeck, int size) {
        //make sure the deck is empty
        deck.Clear();

        //set the size of loaded deck
        deckSize = size;

        // loop through hand first then move on to the deck
        for (int i = 0; i < deckSize; i++) {
            if (i < 5) {
                hand[i] = passedDeck[i];
                currentHandSize++;
            } else {
                addCardToDeck(passedDeck[i]);
            }
        }
        return true;
    }


    /// <summary>
    /// takes the current deck state and loads it in to an array for storage (think exiting the game)
    /// </summary>
    /// <returns> an array with the first five slot representing the hand and the next ten representing the deck </returns>
    public List<GameObject> storeDeck() {
        // array that will be used to send deck information out of the system
        List<GameObject> returnList = new List<GameObject>();

        // loop through hand first then move on to the deck
        for (int i = 0; i < deckSize; i++) {
            if (i < 5) {
                returnList.Add(hand[i]);
                currentHandSize--;
            } else {
                returnList.Add(deck.Dequeue());
                currentDeckSize--;
            }
        }
        return returnList;
    }

    /// <summary>
    /// removes the used card from its hand slot, and in future will trigger the cards effect, is given no paramiters as it uses the currently selected hand slot
    /// </summary>
    public void useCard() {
        //stuff here to trigger card script
        // put card in discard and remove from hand
        discard.Add(hand[currentHandIndex]);
        hand[currentHandIndex] = null;
        //reflect change in hand size
        currentHandSize--;
    }

    //to add tests 
}
