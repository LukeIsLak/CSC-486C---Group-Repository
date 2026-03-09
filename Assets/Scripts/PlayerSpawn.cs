using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public string playerTag;

    void Start()
    {
        DoSpawn();
    }
    public void DoSpawn()
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        cc.enabled = false;

        player.transform.position = transform.position;
        player.transform.forward = transform.forward;

        if (cc != null)
        cc.enabled = true;
    }


}
