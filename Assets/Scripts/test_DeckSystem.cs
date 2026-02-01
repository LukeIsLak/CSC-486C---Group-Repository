using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class test_deckSystem : MonoBehaviour
{

    public DeckSystems deckSystem;
    public TMP_Text output;
    List<GameObject> testDeck;

    // Start is called before the first frame update
    void Start()
    {
        GameObject one = new GameObject("Card1");
        GameObject two = new GameObject("Card2");
        GameObject three = new GameObject("Card3");
        GameObject four = new GameObject("Card4");
        GameObject five = new GameObject("Card5");
        GameObject six = new GameObject("Card6");
        GameObject seven = new GameObject("Card7");
        GameObject eight = new GameObject("Card8");
        GameObject nine = new GameObject("Card9");
        GameObject ten = new GameObject("Card10");
        GameObject eleven = new GameObject("Card11");
        GameObject twelve = new GameObject("Card12");
        GameObject thirteen = new GameObject("Card13");
        GameObject fourteen = new GameObject("Card14");
        GameObject fifteen = new GameObject("Card15");

        testDeck = new List<GameObject> { one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen };
    }


    public void TestshuffleInc() {
        deckSystem.loadDeck(testDeck, 15);
        deckSystem.shuffleIncHand();

        string hand = "";
        string deck = "";
        List<GameObject> resultList = deckSystem.storeDeck();

        for (int i = 0; i < resultList.Count; i++) {
            if (i < 5) {
                hand += resultList[i].name + ", ";
            }
            else {
                deck += resultList[i].name + ", ";
            }
        }

        output.text = "hand : " + hand + " Deck : " + deck;
    }

    public void Testloaddeck() {
        deckSystem.loadDeck(testDeck, 15);

        string hand = "";
        string deck = "";

        for (int i = 0; i < testDeck.Count; i++) {
            if (i < 5) {
                hand += deckSystem.hand[i].name + ", ";
            } else {
                deck += deckSystem.drawCard(0).name + ", "; // note this does change what the hand is in the system and remove objects from the deck
            }
        }

        output.text = "hand : " + hand + " Deck : " + deck;
    }

    public void Teststoredeck() {
        deckSystem.loadDeck(testDeck, 15);

        string hand = "";
        string deck = "";
        List<GameObject> resultList = deckSystem.storeDeck();

        for (int i = 0; i < resultList.Count; i++) {
            if (i < 5) {
                hand += resultList[i].name + ", ";
            } else {
                deck += resultList[i].name + ", ";
            }
        }

        output.text = "hand : " + hand + " Deck : " + deck;
    }

    public void TestshuffleExc()
    {
        deckSystem.loadDeck(testDeck, 15);
        deckSystem.shuffleExcHand();

        string hand = "";
        string deck = "";
        List<GameObject> resultList = deckSystem.storeDeck();

        for (int i = 0; i < resultList.Count; i++)
        {
            if (i < 5)
            {
                hand += resultList[i].name + ", ";
            }
            else
            {
                deck += resultList[i].name + ", ";
            }
        }

        output.text = "hand : " + hand + " Deck : " + deck;
    }

    public void Testusecard() {
        deckSystem.loadDeck(testDeck, 15);

        deckSystem.useCard();

        string hand = "";
        string deck = "";
        List<GameObject> resultList = deckSystem.storeDeck();

        for (int i = 0; i < resultList.Count; i++)
        {
            if (i < 5)
            {
                hand += resultList[i].name + ", ";
            }
            else
            {
                deck += resultList[i].name + ", ";
            }
        }

        output.text = "hand : " + hand + " Deck : " + deck;
    }

}
