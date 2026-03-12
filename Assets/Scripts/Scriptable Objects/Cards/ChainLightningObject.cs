using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningObject : MonoBehaviour
{

    private float damage = 10f;
    private int chainNumber = 0;
    private int maxChain = 5;
    private float ttl = 5f;

    public HashSet<EnemyInterface> alreadyHit = new HashSet<EnemyInterface>();

    [SerializeField] private ChainLightningObject ChainLightningPrefab;


    private GameObject player;


    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        //Create a hashset to store the enemies already hit in the chain
        player = GameObject.FindWithTag("Player");


        if (enemy != null){
            enemy.Hit(damage);
            ContinueChain(alreadyHit, enemy);
            Destroy(this.gameObject);

        }
        else 
        {
            enemy = other.GetComponentInParent<EnemyInterface>();
            enemy.Hit(damage);
            ContinueChain(alreadyHit, enemy);
            Destroy(this.gameObject);

        }
    }

    private void ContinueChain(HashSet<EnemyInterface> alreadyHit, EnemyInterface enemy)
    {
        //While the chain hasn't reached its cap, 
        alreadyHit.Add(enemy);
        if (chainNumber < maxChain)
            {
                ChainLightningObject next = Instantiate(ChainLightningPrefab, enemy.transform.position + Vector3.up * 1.5f, enemy.transform.rotation);
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
