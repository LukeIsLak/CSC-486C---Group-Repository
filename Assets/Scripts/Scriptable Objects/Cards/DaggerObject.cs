using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerObject : MonoBehaviour
{

    [SerializeField] public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        //On hit, get the enemy data and deal the damage
        if (enemy != null){
            enemy.Hit(damage);
            if (enemy != null)
            {
                //If the enemy lives, do the bleeding effect as per design document
                StartCoroutine(bleedEffect(enemy));
            }
            Destroy(this.gameObject);
            
        }
        else {
            enemy = other.GetComponentInParent<EnemyInterface>();
            enemy.Hit(damage);
            if (enemy != null)
            {
                StartCoroutine(bleedEffect(enemy));
            }
            Destroy(this.gameObject);
        }
    }

    private IEnumerator bleedEffect(EnemyInterface enemy)
    {
        float length = 10f;
        float amount = 0f;
        float bleedDamage = 5f;
        float timeDifference = 1f;

        while (amount < length){


            if (enemy != null){
                enemy.Hit(bleedDamage);
                break;
            }
            yield return new WaitForSeconds(timeDifference);

            amount += timeDifference;
        }
    }

}
