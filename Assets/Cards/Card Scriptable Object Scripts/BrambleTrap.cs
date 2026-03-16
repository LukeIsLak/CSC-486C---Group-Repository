using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleTrap : MonoBehaviour
{
    [SerializeField] private float slowAmount = 5f;
    [SerializeField] private float damage = 2f;

    [SerializeField] private float ttl = 5f;

    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    //Change this function later to prevent double hits
    void OnTriggerStay(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy != null){
                enemy.Hit(damage);
            }
            else {
                enemy = other.GetComponentInParent<EnemyInterface>();
                if (enemy != null){
                    enemy.Hit(damage);
                }
            }

        //Implement slow and damage
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }
}
