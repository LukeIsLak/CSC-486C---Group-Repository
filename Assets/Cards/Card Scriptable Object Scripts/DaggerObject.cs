using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerObject : MonoBehaviour
{

    [SerializeField] public float damage = 10f;

    [SerializeField] private DamageOverTime effect;

    [SerializeField] private float ttl = 5f;
    private FMOD.Studio.EventInstance daggerHit;



    void Start(){
        StartCoroutine(timeToLive(ttl));
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayDaggerHit();
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

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }

    private void PlayDaggerHit() 
    {
        daggerHit = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerAttacks/SoftSurfaceStrike");
        daggerHit.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        daggerHit.start();
        daggerHit.release();
    }
}
