using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using Random=UnityEngine.Random;
// idea of this system being its created at the begining of a encounter and stays present throughout
public class DeckSystems : MonoBehaviour
{
    // holds GameObject reference/information of what cards are currently in the players hand
    // will be updated by drawcard()
    public List<CardInstance> hand = new();
    
    // signifys the card the player currently has selected
    public int currentHandIndex = 0;

    //constant for accesing specifc hand slots
    const int HANDSLOT1INDEX = 0;
    const int HANDSLOT2INDEX = 1;
    const int HANDSLOT3INDEX = 2;
    const int HANDSLOT4INDEX = 3;
    const int HANDSLOT5INDEX = 4;

    // const to help define max and min hand size
    public const int MAXHANDSIZE = 5;
    const int EMPTYHANDSIZE = 0;

    // The queue functions as the deck, with dequeue being equivelent to drawing a card, enqueue would be the same as putting a card back in to the deck at the bottom
    public Queue<CardInstance> deck = new Queue<CardInstance>();

    // used out of combat (total deck size)
    public int deckSize;

    // used in combat (cards left in deck during level, ex. doesnt not include hand)
    public int currentDeckSize;

    //store any card that has been used
    public List<CardInstance> discard = new List<CardInstance>();
    
    public event Action OnHandSelectionChanged;
    public event Action OnHandContentsChanged;
    private void NotifyHandSelectionChanged() => OnHandSelectionChanged?.Invoke();
    private void NotifyHandContentsChanged() => OnHandContentsChanged?.Invoke();
    private int nextUid = 0;
    
    public PlayerInventory inventory;

    const float TIMETODRAWNEWCARD = 5f;
    int cardsToDraw = 0;
    bool drawFlag = false;

    // Update is called once per frame, will check if the player hand is empty, if that is the case then fill back up to 5 if possible
    void Update()
    {

    }

    void Start(){
        //Debug.Log("done");
        loadDeck(inventory.playerdeck);
    }

    /// <summary>
    /// adds the given card to the back of the deck, 
    /// ASSUMPTION: when a card is colected the check for deck size happens, this function is for adding a used card back in to the deck
    /// </summary>
    /// <param name="card"></param>
    public void addCardToDeck(CardInstance card) {
        // add card to deck
        deck.Enqueue(card);
        currentDeckSize++;
    }

    /// <summary>
    /// draws the next card in the deck queue, will assign that card to the hand slot it will ocupy and will return null if the handslot number give is invalid
    /// </summary>
    /// <param name="handslot"></param>
    /// <returns> null if handslot is invalid (less than 0 or greater than 5) or the card draw (GameObject) </returns>
    public CardInstance drawCard()
    {
        if (deck.Count == 0) return null;

        if(hand.Count >= MAXHANDSIZE) return null;
        // remove first card from deck
        CardInstance card = deck.Dequeue();

        //// check handslot is valid
        //if (handslot < 0 || handslot > 4) { 
        //    Debug.Log("error wrong value used on drawCard, value should be between 0-4");
        //    return null;
        //}
        currentDeckSize--;
        hand.Add(card);
        NotifyHandContentsChanged();
        // return the draw card for use
        return card;
    }

    /// <summary>
    /// will suffle the player hand but will exclude any card assigned to the player hand, insperation: https://en.wikipedia.org/wiki/Fisher�Yates_shuffle
    /// </summary>
    public void shuffleExcHand() {
        //temporary list to aid in suffeling (abbility to pull specific indexes) 
        List<CardInstance> sortingList = new List<CardInstance>();
        int listSize = currentDeckSize;
        currentDeckSize = 0;

        //empty the deck in to the sorting list
        for (int i = 0; i < listSize; i++) { 
            sortingList.Add(deck.Dequeue());
        }

        //randomly select a index and then put that card in to the deck
        int index = 0;
        while (listSize != 0) {
            index = UnityEngine.Random.Range(0, listSize);
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

        //hand now empty
        hand.Clear();

        //shuffle the deck now it has the players hand in it
        shuffleExcHand();

        //redraw the hand
        drawCard();
        drawCard();
        drawCard();
        drawCard();
        drawCard();
    }

    /// <summary>
    /// will take an array containing cards and add the deck in to the system filling out both the hand and deck
    /// </summary>
    /// <param name="passedDeck"></param>
    /// <param name="size"></param>
    /// <returns> will return true if sucsesful </returns>
    public bool loadDeck(List<CardInstance> passedDeck) {
        //make sure the deck and discard is empty
        hand.Clear();
        deck.Clear();
        discard.Clear();

        //make sure sizes are reset before loading deck
        deckSize = passedDeck.Count;
        currentDeckSize = 0;
        nextUid = 0;
        //set the size of loaded deck

        // loop through hand first then move on to the deck
        for (int i = 0; i < deckSize; i++) {
            passedDeck[i].uid = nextUid++;
            if (i < MAXHANDSIZE) {
                hand.Add(passedDeck[i]);
            } else {
                addCardToDeck(passedDeck[i]);
            }
        }
        NotifyHandContentsChanged();

        return true;
    }


    /// <summary>
    /// takes the current deck state and loads it in to an array for storage (think exiting the game)
    /// </summary>
    /// <returns> an array with the first five slot representing the hand and the next ten representing the deck </returns>
    public CardInstance[] storeDeck() {
        // array that will be used to send deck information out of the system
        CardInstance[] returnArray = new CardInstance[currentDeckSize + hand.Count];

        // loop through hand first then move on to the deck
        for (int i = 0; i < (currentDeckSize + hand.Count); i++) { 
            if (i < hand.Count) {
                returnArray[i] = hand[i];
                
            } else {
                returnArray[i] = deck.Dequeue();
            }
        }

        currentDeckSize = 0;
        return returnArray;
    }

    /// <summary>
    /// removes the used card from its hand slot, and in future will trigger the cards effect, is given no paramiters as it uses the currently selected hand slot
    /// </summary>
    public void useCard() {
        //stuff here to trigger card script
        if (hand.Count == 0) return;
        if(currentHandIndex < 0 || currentHandIndex >= hand.Count) return;

        //call needed card function
        StartCoroutine(hand[currentHandIndex].cardData.Play(hand[currentHandIndex].cardData));

        // put card in discard and remove from hand

        discard.Add(hand[currentHandIndex]);
        hand.RemoveAt(currentHandIndex);
        //reflect change in hand size

        if(hand.Count == 0 ) currentHandIndex = 0;
        else if(currentHandIndex >=  hand.Count) currentHandIndex = hand.Count - 1;

        NotifyHandContentsChanged();

        cardsToDraw++;
        if (!drawFlag){
            drawFlag = true;
            StartCoroutine(newCardTimer());
        }
        
    }

    /// <summary>
    /// Assumes that the discard pill is checked before being called
    /// takes an amount of cards from discard pile (randomly) and puts them in to the players deck, player deck will be shuffled after
    /// </summary>
    /// <param name="amount"></param>
    public void drawFromDiscard(int amount) {
        int temp;
        int discardSize = discard.Count;
        if (amount > discardSize) {
            temp = discardSize;
        } else {
            temp = amount;
        }

        int index = 0;
        
        for (int i = 0; i < temp; i++) {
            index = Random.Range(0, discardSize);
            addCardToDeck(discard[index]);
            discard.RemoveAt(index);
            discardSize--;
        }

        shuffleExcHand();
    }

    /// <summary>
    /// assumes a check that the deck has enough cards for this has happend and that the number is 5 or under
    /// will swap rwo cards in playes hand with two random cards in their deck
    /// </summary>
    /// <param name="amount"></param>
    public void swapcards(int amount) {
        List<int> indexsHand = new List<int>();
        List<int> indexsDeck = new List<int>();
        List<int> avalibleHandSlots = new List<int>();
        List<int> avalibleDeckSlots = new List<int>();
        List<CardInstance> deckList = new List<CardInstance>();

        if(hand.Count < 2 || deck.Count < 2){ // protection from index errors
            if (hand.Count < deck.Count) {
                amount = hand.Count;
            } else {
                amount = deck.Count;
            }
        }

        // get a list of numbers corisponding to card slots 
        for (int i = 0; i < hand.Count; i++) {
            avalibleHandSlots.Add(i);
        }
        for (int i = 0; i < currentDeckSize; i++){
            avalibleDeckSlots.Add(i);
        }

        // pick handslots randomly to swap cards with
        int temp;
        for (int i = 0; i < amount; i++) {
            temp = Random.Range(0, avalibleHandSlots.Count);
            indexsHand.Add(avalibleHandSlots[temp]);
            avalibleHandSlots.RemoveAt(temp);

            temp = Random.Range(0, avalibleDeckSlots.Count);
            indexsDeck.Add(avalibleDeckSlots[temp]);
            avalibleDeckSlots.RemoveAt(temp);
        }

        // pick random cards in deck to swap
        for (int i = 0; i < currentDeckSize; i++){
            deckList.Add(deck.Dequeue());
        }
        //clear up removing entire deck
        currentDeckSize = 0;

        CardInstance swapper;
        for (int i = 0; i < amount; i++) {
            swapper = hand[indexsHand[i]];
            hand[indexsHand[i]] = deckList[indexsDeck[i]];
            deckList[indexsDeck[i]] = swapper;
        }

        while (deckList.Count > 0){
            addCardToDeck(deckList[0]);
            deckList.RemoveAt(0);
        }

        NotifyHandContentsChanged();
    }

    private void ChangeHandIndex(int direction)
    {
        if(hand.Count == 0) return;
        currentHandIndex = (currentHandIndex + direction + hand.Count) % hand.Count;
        NotifyHandSelectionChanged();
    }
 

    public void OnUseCard(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            useCard();
        }
    }

    public void OnSwitchCard(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        float value = context.ReadValue<float>();

        if (value > 0.1f)
        {
            ChangeHandIndex(1);
        }
        else if (value < -0.1f)
        {
            ChangeHandIndex(-1);
        }
    }

    IEnumerator newCardTimer() {
        
        if (deck.Count > 0){
            yield return new WaitForSeconds(TIMETODRAWNEWCARD);
            drawCard();
        }

        cardsToDraw--;

        if (cardsToDraw > 0){
            StartCoroutine(newCardTimer());
        }

        //can still cause some issues with consiten uses of cards ex. 2 then 3 sec break then 2. look in to reseting this after all coroutines are finished
        drawFlag = false;
    }

}
