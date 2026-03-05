using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSystem : MonoBehaviour
{
    public float partialRewardRatio;
    public float fullRewardTime;

    public List<GameObject> rewardChests; // Assign via inspector.


    // Internal reference
    public int totalRooms;
    public int roomsCleared;
    public bool timeFailed;

    public void Initialize()
    {
        timeFailed = false;
        if (partialRewardRatio == 0.0f) DoChestSpawn();
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
    public void IncrementTotalRooms()
    {
        totalRooms++;
    }
    public void RoomCleared()
    {
        roomsCleared++;
        if (roomsCleared == (int) (totalRooms * partialRewardRatio)) DoChestSpawn();
        if (roomsCleared == totalRooms)
        {
            DoChestSpawn();
            if (!timeFailed) DoChestSpawn();
        }
    }

    public void DoChestSpawn()
    {   
        if (rewardChests.Count == 0) return;
        GameObject curChest = rewardChests[0];
        rewardChests.RemoveAt(0);
        curChest.SetActive(true);
    }
}
