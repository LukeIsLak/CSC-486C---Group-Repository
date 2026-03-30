using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Dungeon/RoomShapeVariant")]
public class RoomShapeVariant : ScriptableObject
{
    public List<GameObject> baseShapes;
    // 0 = SINGLE
    // 1 = DOUBLEI
    // 2 = DOUBLEL
    // 3 = TRIPLE
    // 4 = QUAD

    public List<GameObject> regularObjects;
    public List<GameObject> trapObjects;
}
