using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomScript : MonoBehaviour
{
    public GameEvent AddToRoomCountEvent;
    public bool isTrapRoom;

    // If we contain everything needed to make each room type function in a prefab, 
    // We can just instantiate things as needed.
    public GameObject trapRoomObjects; 
    public GameObject regularRoomObjects;
    public void Initialize()
    {
        AddToRoomCountEvent.Raise();
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
        return;
    }

    private void RegularRoomInit()
    {
        // Create necessary objects for regular room functionality
        return;
    }


}
