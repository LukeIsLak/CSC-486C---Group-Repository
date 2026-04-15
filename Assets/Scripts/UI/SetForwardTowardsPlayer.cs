using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetForwardTowardsPlayer : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        Vector3 direction = player.transform.position - transform.position;
        transform.forward = direction;
    }
}
