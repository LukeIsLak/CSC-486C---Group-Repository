using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public Cards cardData;

    //runtime state
    public int uid; // for ui tracking 
    public CardInstance(Cards cardData, int uid)
    {
        this.cardData = cardData;
        this.uid = uid;
    }
}
