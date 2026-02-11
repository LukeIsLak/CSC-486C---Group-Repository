using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create a class to hold the information for the cards
public abstract class Cards : ScriptableObject
{
    public string name = null;
    public Sprite image = null;


    public abstract IEnumerator Play(Cards card);
}
