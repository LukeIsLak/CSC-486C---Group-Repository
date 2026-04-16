using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningObject : MonoBehaviour
{

    [SerializeField] private float damage = 10f;
    [SerializeField] private int chainNumber = 0;
    [SerializeField] private int maxChain = 5;
    [SerializeField] private float ttl = 5f;

    public HashSet<EnemyInterface> alreadyHit = new HashSet<EnemyInterface>();

    [SerializeField] private ChainLightningObject ChainLightningPrefab;


    private GameObject player;


    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) {
            EnemyInterface enemy = other.GetComponent<EnemyInterface>();
            player = GameObject.FindWithTag("Player");
            if (enemy == null) enemy = other.GetComponentInParent<EnemyInterface>();
            if (enemy != null && !alreadyHit.Contains(enemy))
            {
                alreadyHit.Add(enemy); // Add before chaining!
                Vector3 spawn = enemy.transform.position;
                enemy.Hit(damage);
                ContinueChain(alreadyHit, enemy, spawn);
                Destroy(this.gameObject);
            }
        }
        else {
            Destroy(this.gameObject);
        }
    }

    private void ContinueChain(HashSet<EnemyInterface> alreadyHit, EnemyInterface enemy, Vector3 spawn)
    {
        Debug.Log("Something isnt right here2");
        //While the chain hasn't reached its cap, 
        if (enemy != null) alreadyHit.Add(enemy);
        if (chainNumber < maxChain)
            {
                ChainLightningObject next = Instantiate(ChainLightningPrefab, spawn, Quaternion.identity);
                next.alreadyHit = alreadyHit;
                next.chainNumber = chainNumber + 1;
                HomingSystem homing = next.GetComponent<HomingSystem>();
                homing.ignoreEnemies = alreadyHit;
                homing.Initialize();
            }
    }

    
    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }

}
