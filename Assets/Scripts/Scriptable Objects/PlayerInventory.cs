using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public Cards[] tempDeck = new Cards[15];
    public List<Cards> allCards;
    // Start is called before the first frame update
    void Awake()
    {
        Cards one = allCards[Random.Range(0,allCards.Count)];
        Cards two = allCards[Random.Range(0,allCards.Count)];
        Cards three = allCards[Random.Range(0,allCards.Count)];
        Cards four = allCards[Random.Range(0,allCards.Count)];
        Cards five = allCards[Random.Range(0,allCards.Count)];
        Cards six = allCards[Random.Range(0,allCards.Count)];
        Cards seven = allCards[Random.Range(0,allCards.Count)];
        Cards eight = allCards[Random.Range(0,allCards.Count)];
        Cards nine = allCards[Random.Range(0,allCards.Count)];
        Cards ten = allCards[Random.Range(0,allCards.Count)];
        Cards eleven = allCards[Random.Range(0,allCards.Count)];
        Cards twelve = allCards[Random.Range(0,allCards.Count)];
        Cards thirteen = allCards[Random.Range(0,allCards.Count)];
        Cards fourteen = allCards[Random.Range(0,allCards.Count)];
        Cards fifteen = allCards[Random.Range(0,allCards.Count)];

        tempDeck = new Cards[] { one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen, fifteen };

        string temp = "";
        for (int i = 0; i < 15; i++){
            temp += tempDeck[i].name + ", ";
        }
        Debug.Log(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
