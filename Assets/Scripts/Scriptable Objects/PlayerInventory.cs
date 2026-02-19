using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    
    public int currency;
    public List<CardInstance> playerdeck = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // call this in game manager
    public void InitializeRandomDeck(CardsDatabase cardsDB, int deckSize)
    {
        Debug.Log("im here");
        for (int i = 0; i < deckSize; i++)
        {
            Cards pick = cardsDB.allCards[Random.Range(0, cardsDB.allCards.Count)];
            playerdeck.Add(new CardInstance(pick));
            Debug.Log("doobedo");
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
        if (currency - amount > 0) { 
            currency -= amount;
            return true;
        }
        return false;
    }
}
