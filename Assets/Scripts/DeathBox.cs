using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {

        var enemyComponent = other.GetComponentInParent<EnemyInterface>();
        if (enemyComponent != null) { 
            Debug.Log("Killing out of bounds enemy");
            enemyComponent.KillEnemy();
        }
    }
}
