using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFrostNova : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 1f;
    [SerializeField] private float ttl = 5f;


    private float dmg = 8f;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public void Init(Vector3 direct, Cards card){
        //Needs a direction and damage amount
        dir = direct.normalized;
    }

    void Start(){
        StartCoroutine(timeToLive(ttl));
    }

    void Update(){
        //Update position based on direction and speed
        transform.position += dir * projspeed *Time.deltaTime;
    }

    void OnTriggerEnter(Collider other){
        Impact();
    }

    void Impact(){
        //Using projectile information, deal damage to all objects in the area

        Collider[] impactArea = Physics.OverlapSphere(
            transform.position, dmgradius, enemylayer
        );

        foreach (Collider hit in impactArea)
        {
            EnemyInterface enem = hit.GetComponent<EnemyInterface>();
            if (enem != null){
                //Freeze and do damage
                //enem.Hit(dmg);
            }
            else {
                enem = hit.GetComponentInParent<EnemyInterface>();
                if (enem != null){
                    //Freeze and do damage
                    //enem.Hit(dmg);
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
