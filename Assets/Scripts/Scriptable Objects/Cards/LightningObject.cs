using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningObject : MonoBehaviour
{

    private float damage = 10f;
    private int chainNumber = 0;
    private int maxChain = 5;

    private void OnTriggerEnter(Collider object)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();

        if (enemy != null){
            enemy.Hit(damage);
            StartChain();
        }
        else 
        {
            enemy = other.GetComponentInParent<EnemyInterface>();
            enemy.Hit(damage);
            StartChain();
        }
    }

    private void StartChain()
    {
        HashSet<EnemyInterface> alreadyHit = new HashSet<EnemyInterface>();

        

    }
}
