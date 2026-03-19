using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerObject : MonoBehaviour
{

    [SerializeField] public float damage = 10f;

    [SerializeField] private DamageOverTime effect;


    private void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        //On hit, get the enemy data and deal the damage
        if (enemy != null){
            enemy.Hit(damage, effect.type, effect);
            Destroy(this.gameObject);
            
        }
        else {
            enemy = other.GetComponentInParent<EnemyInterface>();
            enemy.Hit(damage, effect.type, effect);
            Destroy(this.gameObject);
        }
    }
}
