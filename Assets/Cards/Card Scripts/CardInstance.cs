using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public Cards cardData;

    //runtime state
    public int uid; // for ui tracking 
    public bool useable; // to see if the card can be used or not

    public Deck deck; // The deck I belong to

    public CardInstance(Cards cardData, int uid)
    {
        this.cardData = cardData;
        this.uid = uid;
        this.useable = true;
    }
}
