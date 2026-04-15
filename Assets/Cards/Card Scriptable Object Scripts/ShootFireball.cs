using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;
    [SerializeField] private float ttl = 5f;
    [SerializeField] private DamageOverTime effect;
    private bool hasImpact = false;


    private float dmg;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public void Init(Vector3 direct, Fireball card){
        //Needs a direction and damage amount
        dir = direct.normalized;
        dmg = card.dmg;
    }

    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    void Update(){
        //Update position based on direction and speed
        transform.position += dir * projspeed *Time.deltaTime;
    }

    void OnTriggerEnter(Collider other){
        if (!hasImpact) {
            hasImpact = true;
            Impact();
        }
    }

    void Impact(){
        //Using projectile information, deal damage to all objects in the area
        List <EnemyInterface> seenEnemies = new List<EnemyInterface>();
        Collider[] impactArea = Physics.OverlapSphere(
            transform.position, dmgradius, enemylayer
        );

        foreach (Collider hit in impactArea)
        {
            EnemyInterface enem = hit.GetComponent<EnemyInterface>();
            if (enem != null){
                if (!seenEnemies.Contains(enem)) {
                    enem.Hit(dmg, effect.type, effect);
                    seenEnemies.Add(enem);
                }
            }
            else {
                enem = hit.GetComponentInParent<EnemyInterface>();
                if (!seenEnemies.Contains(enem)) {
                    enem.Hit(dmg, effect.type, effect);
                    seenEnemies.Add(enem);
                }
            }
        }

        Destroy(this.gameObject);
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Impact();
    }
}
