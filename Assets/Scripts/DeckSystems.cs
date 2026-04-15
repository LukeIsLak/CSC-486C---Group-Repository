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

    // used in combat (cards left in deck during level, ex. doesnt not include hand)
    public int currentDeckSize;

    //store any card that has been used
    public List<CardInstance> discard = new List<CardInstance>();
    
    public event Action OnHandSelectionChanged;
    public event Action OnHandContentsChanged;
    private void NotifyHandSelectionChanged() => OnHandSelectionChanged?.Invoke();
    private void NotifyHandContentsChanged() => OnHandContentsChanged?.Invoke();
    
    //link to players inventory so system can load the deck in for combat
    public PlayerInventory inventory;

    // vars related to drawing new cards
    const float TIMETODRAWNEWCARD = 5f;
    int cardsToDraw = 0;
    bool drawFlag = false;

    public float delayamount = 0.1f;


    void Start(){
        loadDeck(inventory.activeDeck.contents);
        shuffleIncHand();
    }

    /// <summary>
    /// adds the given card to the back of the deck, 
    /// ASSUMPTION: when a card is collected the check for deck size happens, this function is for adding a used card back in to the deck
    /// </summary>
    /// <param name="card"></param>
    public void addCardToDeck(CardInstance card) {
        // add card to back of deck queue
        deck.Enqueue(card);
        currentDeckSize++;
    }

    /// <summary>
    /// draws the next card in the deck queue, will be added to the hand list
    /// </summary>
    /// <returns> null if deck is empty or if hand size is already at 5 </returns>
    public void drawCard()
    {
        if (deck.Count == 0) return;

        if(hand.Count >= MAXHANDSIZE) return;
        // remove first card from deck
        CardInstance card = deck.Dequeue();

        // update deck size, and add card to hand
        currentDeckSize--;
        hand.Add(card);
        NotifyHandContentsChanged();
    }

    /// <summary>
    /// will suffle the player hand but will exclude any card assigned to the player hand, insperation: https://en.wikipedia.org/wiki/Fisher�Yates_shuffle
    /// </summary>
    public void shuffleExcHand() {
        //temporary list to aid in suffling (ability to pull specific indexes) 
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
        // check to see if there are enough cards in the player hand and deck to have a full hand (5 cards) if not find the highest possible hand value
        int looplength = MAXHANDSIZE;
        if (deck.Count + hand.Count < MAXHANDSIZE){ looplength = deck.Count + hand.Count;}

        // add players hand in to their deck
        for (int i = 0; i < looplength; i++){
            addCardToDeck(hand[i]);
        }

        //empty hand as cards are in deck
        hand.Clear();

        //shuffle the deck now it has the players hand in it
        shuffleExcHand();

        //redraw the hand
        StartCoroutine(drawMult(looplength));
    }

    /// <summary>
    /// will take an array containing cardInstances and add them to in to the hand first then, the players deck
    /// </summary>
    /// <param name="passedDeck"></param>
    /// <returns> will return true if sucsesful </returns>
    public bool loadDeck(List<CardInstance> passedDeck) {
        //make sure the deck and discard is empty
        hand.Clear();
        deck.Clear();
        discard.Clear();

        //make sure sizes are reset before loading deck
        currentDeckSize = 0;

        // loop through hand first then move on to the deck
        for (int i = 0; i < passedDeck.Count; i++) {
            // check if cardhas or hasent been used since last rest stop
            if (passedDeck[i].useable) {
                // if hand still has space add card to hand
                if (hand.Count < MAXHANDSIZE) {
                    hand.Add(passedDeck[i]);
                    //Debug.Log("hand" + passedDeck[i].cardData.name);
                } else {
                    addCardToDeck(passedDeck[i]);
                    //Debug.Log("deck" + passedDeck[i].cardData.name);
                }
            }
        }
        // NotifyHandContentsChanged();

        return true;
    }


    /// <summary>
    /// takes the current deck state and loads it in to an array for storage (think exiting the game)
    /// </summary>
    /// <returns> an array with the first five slot representing the hand and the rest representing the deck </returns>
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

        //deck should be considered empty now so set size to match
        currentDeckSize = 0;
        return returnArray;
    }

    /// <summary>
    /// removes the used card from its hand slot, and in future will trigger the cards effect, is given no paramiters as it uses the currently selected hand slot
    /// </summary>
    public void useCard() {
        //checks to make sure process is valid
        if (hand.Count == 0) return;
        if(currentHandIndex < 0 || currentHandIndex >= hand.Count) return;

        //get selected card
        CardInstance curCard = hand[currentHandIndex];
        

        // put card in discard, remove from hand and set the cards usabilty to false
        string cardname = hand[currentHandIndex].cardData.name;
        hand[currentHandIndex].useable = false;
        discard.Add(hand[currentHandIndex]);
        hand.RemoveAt(currentHandIndex);

        //call needed card function 
        StartCoroutine(DelayCardPlay(curCard));

        // reset currently selected card
        if(hand.Count == 0 ) currentHandIndex = 0;
        else if(currentHandIndex >=  hand.Count) currentHandIndex = hand.Count - 1;

        NotifyHandContentsChanged();

        // due to recall and greed messing with drawing new cards they handle new card draws in their function code, so code below shouldnt be called for them
        if (cardname != "Recall" && cardname != "Greed"){
            // add a card to draw queue, if a draw currently isnt in process start that process
            cardsToDraw++;
            if (!drawFlag){
                drawFlag = true;
                StartCoroutine(newCardTimer());
            }
        } 
        
    }

    private IEnumerator DelayCardPlay(CardInstance curCard) {
        yield return new WaitForSeconds(delayamount);
        StartCoroutine(curCard.cardData.Play(curCard.cardData));
    }

    /// <summary>
    /// Assumes that the discard pile is checked before being called
    /// takes an amount of cards from discard pile (randomly) and puts them in to the players deck, player deck will be shuffled after
    /// </summary>
    /// <param name="amount"></param>
    public void drawFromDiscard(int amount) {
        //figure out if the amount requested can be drawn from the discard pile
        int amountToDraw;
        int discardSize = discard.Count;
        if (amount > discardSize) {
            amountToDraw = discardSize;
        } else {
            amountToDraw = amount;
        }

        //var to store random choice of discarded card
        int index = 0;
        
        // choose random discarded cards till the amount requested has been fufilled, these cards end up in the player deck
        for (int i = 0; i < amountToDraw; i++) {
            index = Random.Range(0, discardSize);
            addCardToDeck(discard[index]);
            discard.RemoveAt(index);
            discardSize--;
        }

        // shuffle just the players deck, so added cards are spread out and not all at the end
        shuffleExcHand();
    }

    /// <summary>
    /// will swap an amount of cards in players hand with an amount of random cards in their deck
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
        int selectedIndex;
        for (int i = 0; i < amount; i++) {
            selectedIndex = Random.Range(0, avalibleHandSlots.Count);
            indexsHand.Add(avalibleHandSlots[selectedIndex]);
            avalibleHandSlots.RemoveAt(selectedIndex);

            selectedIndex = Random.Range(0, avalibleDeckSlots.Count);
            indexsDeck.Add(avalibleDeckSlots[selectedIndex]);
            avalibleDeckSlots.RemoveAt(selectedIndex);
        }

        // move deck in to a list to help with swapping
        for (int i = 0; i < currentDeckSize; i++){
            deckList.Add(deck.Dequeue());
        }
        //clear up removing entire deck
        currentDeckSize = 0;

        //swap cards in deck and hand
        CardInstance swapper;
        for (int i = 0; i < amount; i++) {
            swapper = hand[indexsHand[i]];
            hand[indexsHand[i]] = deckList[indexsDeck[i]];
            deckList[indexsDeck[i]] = swapper;
        }

        //put deck back in to a queue
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
            ChangeHandIndex(-1);
        }
        else if (value < -0.1f)
        {
            ChangeHandIndex(1);
        }
    }

    // used to put a card from the player deck in to their hand 
    IEnumerator newCardTimer() {
        // check deck has enough cards to draw one
        if (deck.Count > 0){
            // set the card timer (crad takes time to draw)
            yield return new WaitForSeconds(TIMETODRAWNEWCARD);
            // double check that card slot hasnt been filled
            if (cardsToDraw > 0){
                drawCard();
            }
        }

        cardsToDraw--;

        // if more cards needed to draw call function again
        if (cardsToDraw > 0){
            StartCoroutine(newCardTimer());
        }

        // if no more cards to draw set the draw flag to false
        if (cardsToDraw == 0){
            drawFlag = false;
        }
    }

    // add last used card back in to the players hand
    public void recallCard(){
        // check there is a card in discard that can be drawn (e.x. not the recall that was just used)
        if (discard.Count > 1){
            hand.Add(discard[discard.Count - 2]);

            NotifyHandContentsChanged();

            discard.RemoveAt(discard.Count - 2);
        } else { // if no card able to be moved from discard in to hand call the draw card function to get one from the deck
            cardsToDraw++;
            if (!drawFlag){
                drawFlag = true;
                StartCoroutine(newCardTimer());
            }
        }
    }

    // draws an amount of cards from the players deck instatly
    public void GreedDrawCards(int amount){
        // check if amount plus current hand is larger than hand, if not make amount smaller so it isnt larger
        if (hand.Count + amount > MAXHANDSIZE){ 
            amount = MAXHANDSIZE - hand.Count;
        }

        // draw an amount of cards
        for (int i = 0; i < amount; i++){
            drawCard();
        }
        NotifyHandContentsChanged();

        //check if there should be a card draws
        if (hand.Count < MAXHANDSIZE){ 
            cardsToDraw = MAXHANDSIZE - hand.Count;

            if (!drawFlag){
                drawFlag = true;
                StartCoroutine(newCardTimer());
            }
        }
    }

    public IEnumerator drawMult(int looplength) {
        for (int i = 0; i < looplength; i++) {
            drawCard();
            yield return new WaitForSeconds(0.3f);
            NotifyHandContentsChanged();
        }
    }
}
