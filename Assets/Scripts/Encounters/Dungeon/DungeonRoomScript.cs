using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomScript : MonoBehaviour
{
    // If we contain everything needed to make each room type function in a prefab, 
    // We can just instantiate things as needed.
    [Header("References")]
    public RandomContext randomContext;
    public GameEvent AddToRoomCountEvent;
    public GameObject trapRoomObjects; 
    public GameObject regularRoomObjects;
<<<<<<< Updated upstream
=======
    public List<GameObject> roomVariants;
    
    [Header("Properties")]
    public bool isTrapRoom;

    public void Start()
    {
        if (roomVariants.Count == 0)
        {
            Debug.LogWarning("No prefab provided for dungeon room");
            return;
        }

        int r = randomContext.rnd.NextInt(roomVariants.Count);
        Instantiate(roomVariants[r], transform);

    }
>>>>>>> Stashed changes
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
