using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleTrap : MonoBehaviour
{
    [SerializeField] private float slowAmount = 5f;
    [SerializeField] private float damage = 2f;

    [SerializeField] private float ttl = 5f;

    [SerializeField] private DamageOverTime DoT;

    [SerializeField] private Stop stopEffect;



    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    //Change this function later to prevent double hits
    void OnTriggerEnter(Collider other)
    {
        List <EnemyInterface> seenEnemies = new List<EnemyInterface>();
        EnemyInterface enemy = other.GetComponent<EnemyInterface>();
        if (enemy != null){
             if (!seenEnemies.Contains(enemy)) {
                enemy.Hit(damage, DoT.type, DoT);
                enemy.Hit(0, stopEffect.type, stopEffect);
                seenEnemies.Add(enemy);

                }
        }
            else {
                enemy = other.GetComponentInParent<EnemyInterface>();
                if (enemy != null){
                    if (!seenEnemies.Contains(enemy)) {
                    enemy.Hit(damage, DoT.type, DoT);
                    enemy.Hit(0, stopEffect.type, stopEffect);
                    seenEnemies.Add(enemy);

                    }
                }
            }

        //Implement slow
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }
}
