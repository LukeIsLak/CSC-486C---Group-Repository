using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
//Create a class to hold all the information for the cards
public class Cards : ScriptableObject
{
    public string name = null;
    public Sprite image = null;

    public string element = null;

    public string attackType = null;

    //public int attackDamage = null;
}
