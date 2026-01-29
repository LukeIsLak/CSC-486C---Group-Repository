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

    const int HANDSLOT1INDEX = 0;
    const int HANDSLOT2INDEX = 1;
    const int HANDSLOT3INDEX = 2;
    const int HANDSLOT4INDEX = 3;

    // The queue functions as the deck, with dequeue being equivelent to drawing a card, enqueue would be the same as putting a card back in to the deck at the bottom
    public Queue<GameObject> deck = new Queue<GameObject>();
    // used out of combat (total deck size)
    public int deckSize;
    // used in combat (cards left in deck during level, ex. doesnt not include hand)
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
            case HANDSLOT1INDEX:
                handSlot1 = card;
                break;
            case HANDSLOT2INDEX:
                handSlot2 = card;
                break;
            case HANDSLOT3INDEX:
                handSlot3 = card;
                break;
            case HANDSLOT4INDEX:
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
        List<GameObject> sortingList = new List<GameObject>();
        int listSize = currentDeckSize;
        currentDeckSize = 0;

        for (int i = 0; i < listSize; i++) { 
            sortingList.Add(deck.Dequeue());
        }

        int index = 0;
        while (listSize != 0) {
            index = Random.Range(0, listSize);
            addCardToDeck(sortingList[index]);
            sortingList.RemoveAt(index);
            listSize--;
        }
    }

    public void shuffleIncHand() {
        addCardToDeck(handSlot1);
        addCardToDeck(handSlot2);
        addCardToDeck(handSlot3);
        addCardToDeck(handSlot4);

        shuffleExcHand();

        drawCard(HANDSLOT1INDEX);
        drawCard(HANDSLOT2INDEX);
        drawCard(HANDSLOT3INDEX);
        drawCard(HANDSLOT4INDEX);
    }

    public bool loadDeck(GameObject[] passedDeck, int size) { 
        deckSize = size;
        for (int i = 0; i < deckSize; i++) {
            switch (i)
            {
                case HANDSLOT1INDEX:
                    handSlot1 = passedDeck[i];
                    break;
                case HANDSLOT2INDEX:
                    handSlot2 = passedDeck[i];
                    break;
                case HANDSLOT3INDEX:
                    handSlot3 = passedDeck[i];
                    break;
                case HANDSLOT4INDEX:
                    handSlot4 = passedDeck[i];
                    break;
                default:
                    addCardToDeck(passedDeck[i]);
                    break;
            }
        }
        return true;
    }

    public GameObject[] storeDeck() {
        GameObject[] returnArray = new GameObject[deckSize];

        for (int i = 0; i < deckSize; i++){
            switch (i)
            {
                case HANDSLOT1INDEX:
                    returnArray[i] = handSlot1;
                    break;
                case HANDSLOT2INDEX:
                    returnArray[i] = handSlot2;
                    break;
                case HANDSLOT3INDEX:
                    returnArray[i] = handSlot3;
                    break;
                case HANDSLOT4INDEX:
                    returnArray[i] = handSlot4;
                    break;
                default:
                    returnArray[i] = deck.Dequeue();
                    break;
            }
        }
        return returnArray;
    }

    //to add shuffle (not including hand), shuffle (including hand) load deck, store deck, tests, 
}
