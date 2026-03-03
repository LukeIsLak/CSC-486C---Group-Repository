using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningObject : MonoBehaviour
{

    private float damage = 10f;
    private int chainNumber = 0;
    private int maxChain = 5;

    [SerializeField] private ChainLightningObject ChainLightningPrefab;


    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        HashSet<EnemyInterface> alreadyHit = new HashSet<EnemyInterface>();
        player = GameObject.FindWithTag("Player");


        if (enemy != null){
            enemy.Hit(damage);
            StartChain(alreadyHit, enemy);
        }
        else 
        {
            enemy = other.GetComponentInParent<EnemyInterface>();
            enemy.Hit(damage);
            if (chainNumber == 0)
            {
                StartChain(alreadyHit, enemy);
            }
            else 
            {
                ContinueChain(alreadyHit, enemy);
            }
        }
    }

    private void StartChain(HashSet<EnemyInterface> alreadyHit, EnemyInterface enemy)
    {
        alreadyHit.Add(enemy);
        ChainLightningObject ChainLightning = Instantiate(ChainLightningPrefab, enemy.transform.position + enemy.transform.forward * 2f, enemy.transform.rotation);

        HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
        homing.Initialize();

        chainNumber += 1;
        

    }

    private void ContinueChain(HashSet<EnemyInterface> alreadyHit, EnemyInterface enemy)
    {
        if (chainNumber < maxChain)
            {
                ChainLightningObject ChainLightning = Instantiate(ChainLightningPrefab, enemy.transform.position + enemy.transform.forward * 2f, enemy.transform.rotation);

                HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
                homing.Initialize();
                chainNumber += 1;
            }
    }
}
