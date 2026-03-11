using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleTrap : MonoBehaviour
{
    private float slowAmount = 5f;
    private float damage = 5f;

    private float ttl = 5f;

    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();

        //Implement slow and damage
    }
    void OnTrigggerExit()
    {
        //Remove slow and damage 
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }
}
