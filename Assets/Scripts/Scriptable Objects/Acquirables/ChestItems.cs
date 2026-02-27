using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Data/Acquirable/ChestItems")]
public class ChestItems : ScriptableObject
{
    public AcquirableList cardAcquirables;
    public AcquirableList goldAcquirables;
    public RandomContext randomContext;

    public Acquirable GetCard()
    {
        return AcquirableList.MakeWeightedChoice(cardAcquirables.items, randomContext);
    }

    public Acquirable GetGold()
    {
        return AcquirableList.MakeWeightedChoice(goldAcquirables.items, randomContext);
    }

    public Acquirable GetAny()
    {
        List<Acquirable> tempJoin = cardAcquirables.items.Concat(goldAcquirables.items).ToList();
        return AcquirableList.MakeWeightedChoice(tempJoin, randomContext);
    }

}
