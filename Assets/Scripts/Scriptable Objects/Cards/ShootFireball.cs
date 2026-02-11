using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootFireball : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;


    private float dmg;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public void Init(Vector3 direct, Fireball card){
        //Needs a direction and damage amount
        dir = direct.normalized;
        dmg = card.dmg;
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
                enem.Hit(dmg);
                //Luke hasn't pushed this yet!
            }
        }

        Destroy(this.gameObject);
    }
}
