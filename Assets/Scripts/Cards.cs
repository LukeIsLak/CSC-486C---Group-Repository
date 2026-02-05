using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Create a class to hold the information for the cards
public class Cards : ScriptableObject
{
    public string name = null;
    public Sprite image = null;

}

[CreateAssetMenu(menuName = "Cards/AttackCards")]
public class AttackCards : Cards
{
    public int attackDamage;
    public string element;
    public string attackType;

}

[CreateAssetMenu(menuName = "Cards/ModifierCards")]
public class ModifierCards : Cards
{
    public string effect;
    public int value;
}