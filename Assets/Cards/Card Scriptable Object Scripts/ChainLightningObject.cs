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

    private FMOD.Studio.EventInstance lightningSound;
    private FMOD.Studio.EventInstance chainLightningSound;




    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayLightningSound();
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        //Create a hashset to store the enemies already hit in the chain
        player = GameObject.FindWithTag("Player");


        if (enemy != null){
            enemy.Hit(damage);
            ContinueChain(alreadyHit, enemy);
            Destroy(this.gameObject);

        if (other.gameObject.CompareTag("Enemy")) {
            EnemyInterface enemy = other.GetComponent<EnemyInterface>();
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

    private void ContinueChain(HashSet<EnemyInterface> alreadyHit, EnemyInterface enemy, Vector3 spawn) {
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

    private void PlayLightningSound() 
    {
        lightningSound = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/chainlightningHit");
        lightningSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        lightningSound.start();
        lightningSound.release();
    }

}
