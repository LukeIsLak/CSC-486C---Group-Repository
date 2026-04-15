using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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

    // Decoration sets per connectivity type
    [Header("Decoration Sets per Connectivity")]
    public List<GameObject> decorationSet0;
    public List<GameObject> decorationSet1;
    public List<GameObject> decorationSet2;
    public List<GameObject> decorationSet3;
    public List<GameObject> decorationSet4;


    [System.NonSerialized]
    public List<List<GameObject>> decorationSets;

    void OnEnable()
    {
        decorationSets = new List<List<GameObject>> {
            decorationSet0,
            decorationSet1,
            decorationSet2,
            decorationSet3,
            decorationSet4
        };
    }
}
