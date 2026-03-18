using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/CardsDatabase")]
public class CardsDatabase : ScriptableObject
{
    public List<Cards> allCards;

}
