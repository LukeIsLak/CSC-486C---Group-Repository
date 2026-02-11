using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class test_deckSystem : MonoBehaviour
{
    ////note all of these test are manal and can be viewed in the decksystem test scene
    //public DeckSystems deckSystem;
    //public TMP_Text output;
    //public Cards[] testDeck;

    ////List of all cards set in the inspector
    //public List<Cards> allCards;

    //// set up mock cards and deck to be used for testing
    //void Start()
    //{
    //    //Create a list of a random cards as the starting deck
    //    Cards one = allCards[Random.Range(0,allCards.Count)];
    //    Cards two = allCards[Random.Range(0,allCards.Count)];
    //    Cards three = allCards[Random.Range(0,allCards.Count)];
    //    Cards four = allCards[Random.Range(0,allCards.Count)];
    //    Cards five = allCards[Random.Range(0,allCards.Count)];
    //    Cards six = allCards[Random.Range(0,allCards.Count)];
    //    Cards seven = allCards[Random.Range(0,allCards.Count)];
    //    Cards eight = allCards[Random.Range(0,allCards.Count)];
    //    Cards nine = allCards[Random.Range(0,allCards.Count)];
    //    Cards ten = allCards[Random.Range(0,allCards.Count)];
    //    Cards eleven = allCards[Random.Range(0,allCards.Count)];
    //    Cards twelve = allCards[Random.Range(0,allCards.Count)];
    //    Cards thirteen = allCards[Random.Range(0,allCards.Count)];
    //    Cards fourteen = allCards[Random.Range(0,allCards.Count)];
    //    Cards fifteen = allCards[Random.Range(0,allCards.Count)];

    //    testDeck = new Cards[] { one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen };
    //}

    ///// <summary>
    ///// tests shuffleing a deck including a player hand
    ///// </summary>
    //public void TestshuffleInc() {
    //    //load deck in to the system and then shuffle it and the hand
    //    deckSystem.loadDeck(testDeck, 15);
    //    deckSystem.shuffleIncHand();
        
    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    //get deck back from system
    //    Cards[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++) {
    //        if (i < 5) {
    //            hand += resultArray[i].name + ", ";
    //        }
    //        else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck;
    //}

    ///// <summary>
    ///// test the system loads a deck with out changing any information
    ///// </summary>
    //public void Testloaddeck() {
    //    // load deck in to the system
    //    deckSystem.loadDeck(testDeck, 15);

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < testDeck.Length; i++) {
    //        if (i < 5) {
    //            hand += deckSystem.hand[i].name + ", ";
    //        } else {
    //            deck += deckSystem.deck.Dequeue().name + ", "; // note this removes objects from the deck
    //        }
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck;
    //}

    ///// <summary>
    ///// test that a deck can be loaded and stored with out any information changing
    ///// </summary>
    //public void Teststoredeck() {
    //    //load deck in to the system
    //    deckSystem.loadDeck(testDeck, 15);

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    //get deck back from system
    //    Cards[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++) {
    //        if (i < 5) {
    //            hand += resultArray[i].name + ", ";
    //        } else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck;
    //}


    ///// <summary>
    ///// test shuffleing with out touching the hand
    ///// </summary>
    //public void TestshuffleExc() {
    //    // load deck in to the system and then shuffle the cards left in the deck
    //    deckSystem.loadDeck(testDeck, 15);
    //    deckSystem.shuffleExcHand();

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    //get deck back from system
    //    Cards[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++) {
    //        if (i < 5) {
    //            hand += resultArray[i].name + ", ";
    //        } else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck;
    //}

    ///// <summary>
    ///// test the use card function (check it when in to the discard and turned the hand value to null)
    ///// </summary>
    //public void Testusecard() {
    //    // load deck in to the system
    //    deckSystem.loadDeck(testDeck, 15);
    //    // tell the system to use a card (will use the card at index 0 as thats the system defalut)
    //    deckSystem.useCard();

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    string discard = "";
    //    //get deck back from system
    //    Cards[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++) {
    //        if (i < 5) {
    //            if (resultArray[i] == null) {
    //                hand += " null ";
    //            } else {
    //                hand += resultArray[i].name + ", ";
    //            }
    //        } else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    // pull information from dicard list
    //    for (int i = 0; i < deckSystem.discard.Count; i++) {
    //        discard += deckSystem.discard[i].name;
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck + " discard : " + discard;
    //}

    //public void Testredraw() {
    //    StartCoroutine(Testredrawco());
    //}

    ///// <summary>
    ///// test the systems ability to redraw a hand once all cards in the previous hand have been used
    ///// </summary>
    ///// <returns></returns>
    //IEnumerator Testredrawco() {
    //    // load deck in to the system then use all cards in players hand
    //    deckSystem.loadDeck(testDeck, 15);
    //    deckSystem.useCard();
    //    deckSystem.useCard();
    //    deckSystem.useCard();
    //    deckSystem.useCard();
    //    deckSystem.useCard();

    //    yield return new WaitForSeconds(1);

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    string discard = "";
    //    //get deck back from system
    //    Cards[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++){
    //        if (i < 5){
    //            if (resultArray[i] == null){
    //                hand += " null ";
    //            } else {
    //                hand += resultArray[i].name + ", ";
    //            }
    //        } else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    // pull information from dicard list
    //    for (int i = 0; i < deckSystem.discard.Count; i++) {
    //        discard += deckSystem.discard[i].name + ", ";
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck + " discard : " + discard;
    //}

    //public void Testnocards() {
    //    StartCoroutine(Testnocardsco());
    //}

    ///// <summary>
    ///// test the system when all cards are used
    ///// </summary>
    ///// <returns></returns>
    //IEnumerator Testnocardsco(){
    //    // load deck in to the system
    //    deckSystem.loadDeck(testDeck, 15);
    //    // use all cards in the player hand 3 times to empty out the deck
    //    for (int i = 0; i < 3; i++) {
    //        deckSystem.useCard();
    //        deckSystem.useCard();
    //        deckSystem.useCard();
    //        deckSystem.useCard();
    //        deckSystem.useCard();
    //        yield return new WaitForSeconds(1);
    //    }

    //    // strings used to put information in to tmp text
    //    string hand = "";
    //    string deck = "";
    //    string discard = "";
    //    //get deck back from system
    //    CardInstance[] resultArray = deckSystem.storeDeck();

    //    // pull information from deck and prep to be outputted
    //    for (int i = 0; i < resultArray.Length; i++) {
    //        if (i < 5) {
    //            if (resultArray[i] == null) {
    //                hand += " null ";
    //            } else {
    //                hand += resultArray[i].name + ", ";
    //            }
    //        } else {
    //            deck += resultArray[i].name + ", ";
    //        }
    //    }

    //    // pull information from dicard list
    //    for (int i = 0; i < deckSystem.discard.Count; i++) {
    //        discard += deckSystem.discard[i].name + ", ";
    //    }

    //    //output result
    //    output.text = "hand : " + hand + " Deck : " + deck + " discard : " + discard;
    //}

}
