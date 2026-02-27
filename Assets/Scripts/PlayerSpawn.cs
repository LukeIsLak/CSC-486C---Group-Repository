using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public string playerTag;
    public void DoSpawn()
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        if (!player) return;
        player.transform.position = transform.position;
        player.transform.forward = transform.forward;
    }

}
