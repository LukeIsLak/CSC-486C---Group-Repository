using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonRoomScript : MonoBehaviour
{
    [Header("References")]
    public RandomContext randomContext;
    public GameEvent AddToRoomCountEvent;

    [Header("Room Config")]
    public RoomShape roomShapeData;
    public int roomBaseShapeIndex; // Not the ideal way but it's quick and easy.
    public bool isTrapRoom;        // set by dungeon manager
    private GameObject baseShape;
    
    private RoomShapeVariant variant;
    public void Awake()
    {
        if (roomShapeData.possibleRooms.Count == 0)
        {
            Debug.LogWarning("No possible rooms provided for dungeon generation!");
            return;
        }

        int r = randomContext.NextInt(roomShapeData.possibleRooms.Count);
        
        variant = roomShapeData.possibleRooms[r];
        baseShape = Instantiate(variant.baseShapes[roomBaseShapeIndex], transform);
    }

    public void Initialize()
    {
        AddToRoomCountEvent.Raise();
        InitByType();
        PlaceDecorations();
    }

    public void InitByType()
    {
        if (isTrapRoom) 
        {
            TrapRoomInit();
            return;
        }
        RegularRoomInit();
    }

    public void SetTrapRoom()
    {
        isTrapRoom = true;
    }

    private void TrapRoomInit()
    {
        if (variant.trapObjects.Count == 0) return;
        int r = randomContext.NextInt(variant.trapObjects.Count);
        Instantiate(variant.trapObjects[r], transform, false);
    }

    private void RegularRoomInit()
    {
        if (variant.regularObjects.Count == 0) return;
        int r = randomContext.NextInt(variant.regularObjects.Count);
        Instantiate(variant.regularObjects[r], transform, false);
    }

    private void PlaceDecorations()
    {
        List<GameObject> set = variant.decorationSets[roomBaseShapeIndex];
        if (set.Count == 0) return;
        int r = randomContext.NextInt(set.Count + 1);
        // Chance for no decoration
        if (r == set.Count) return;
        GameObject decor = Instantiate(set[r], baseShape.transform, false);  
    }
}
