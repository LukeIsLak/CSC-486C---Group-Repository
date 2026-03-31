using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomScript : MonoBehaviour
{
    [Header("References")]
    public RandomContext randomContext;
    public GameEvent AddToRoomCountEvent;
    public List<GameObject> roomVariants;

    [Header("Room Config")]
    public List<GameObject> trapRoomObjects; 
    public List<GameObject> regularRoomObjects;
    public bool isTrapRoom;
    

    public void Start()
    {
        // TO DO: Determine decorated variant to use
        if (roomVariants.Count == 0)
        {
            Debug.LogWarning("No room specified in DungeonRoom");
            return;
        }

        int r = randomContext.NextInt(roomVariants.Count);
        Instantiate(roomVariants[r], transform);
    }

    public void Initialize()
    {
        AddToRoomCountEvent.Raise();
        InitByType();
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
        // Create necessary objects for trap room functionality
        if (trapRoomObjects.Count == 0) return;
        GameObject trapRoomObject = trapRoomObjects[randomContext.NextInt(trapRoomObjects.Count)];
        Instantiate(trapRoomObject, transform);
    }

    private void RegularRoomInit()
    {
        // Create necessary objects for regular room functionality
        if (regularRoomObjects.Count == 0) return;
        GameObject regularRoomObject = regularRoomObjects[randomContext.NextInt(regularRoomObjects.Count)];
        Instantiate(regularRoomObject, transform);
    }
}
