using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFrostNova : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 1f;
    [SerializeField] private float ttl = 5f;
    [SerializeField] private Freeze effect;
    public GameObject explosionEffect;
    private bool hasImpact = false;


    [SerializeField] private float dmg = 1f;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;
    private FMOD.Studio.EventInstance explosion;



    public void Init(Vector3 direct, FrostNova card){
        //Needs a direction and damage amount
        dir = direct.normalized;
        projspeed = card.projspeed;
        dmgradius = card.dmgradius;
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
        PlayExplosion();
        List <EnemyInterface> seenEnemies = new List<EnemyInterface>();
        Collider[] impactArea = Physics.OverlapSphere(
            transform.position, dmgradius, enemylayer
        );

        foreach (Collider hit in impactArea)
        {
            EnemyInterface enem = hit.GetComponent<EnemyInterface>();
            if (enem != null){
                //Freeze and do damage
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

        GameObject explosion = Instantiate(explosionEffect);
        explosion.transform.position = transform.position;
        Destroy(this.gameObject);
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Impact();
    }

    private void PlayExplosion() 
    {
        explosion = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/frostnovaHit");
        explosion.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        explosion.start();
        explosion.release();
    }
}
