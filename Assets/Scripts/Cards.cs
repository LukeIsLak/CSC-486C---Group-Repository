using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create a class to hold the information for the cards
public abstract class Cards : ScriptableObject
{
    public string name = null;
    public Sprite image = null;

    public abstract void Play(Cards card);
}

[CreateAssetMenu(menuName = "Cards/AttackCards")]
public class AttackCards : Cards
{
    public int attackDamage;
    public string element;
    public string attackType;

    public override void Play(Cards card){
        
    }
}

[CreateAssetMenu(menuName = "Cards/ModifierCards")]
public class ModifierCards : Cards
{
    public string effect;
    public int value;

    public override void Play(Cards card){

    }
}