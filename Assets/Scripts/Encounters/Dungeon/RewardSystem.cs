using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public float partialRewardRatio;
    public float fullRewardTime;

    public List<GameObject> rewardChests; // Assign via inspector.

    // Internal reference
    private int totalRooms;
    private int roomsCleared;
    private bool timeFailed;

    public void Initialize()
    {
        timeFailed = false;
<<<<<<< Updated upstream
        // Hide chests if needed
        // start timer
    }
=======
        if (partialRewardRatio == 0.0f) DoChestSpawn();
        StartChestTimer();
        // Hide chests if needed
        // start timer
    }

    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            RoomCleared();
        }
    }
    */

    private void StartChestTimer()
    {
        StartCoroutine(DoChestTimer());
    }

    private IEnumerator DoChestTimer()
    {
        yield return new WaitForSeconds(fullRewardTime);
        timeFailed = true;
    }
>>>>>>> Stashed changes
    public void IncrementTotalRooms()
    {
        totalRooms++;
    }
    public void RoomCleared()
    {
        roomsCleared++;
        if (roomsCleared >= totalRooms * partialRewardRatio) DoChestSpawn();
        if (roomsCleared == totalRooms)
        {
            DoChestSpawn();
            if (!timeFailed) DoChestSpawn();
        }
    }

    public void DoChestSpawn()
    {
        // Reveal one chest from the list. There will be 3, and DoChestSpawn can only be done at most 3 times.
        // The list contains referenced to instantiated chests that are currently not visible
        return;
    }
}
