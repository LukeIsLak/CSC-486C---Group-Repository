using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAoEProjectile : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;


    private int dmg;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public void Init(Vector3 direct, AttackCards card){
        //Needs a direction and damage amount
        dir = direct.normalized;
        dmg = card.attackDamage;
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
            Enemy enem = hit.GetComponent<Enemy>();
            if (enem != null){
                //enem.OnHit(dmg);
                //Luke hasn't pushed this yet!
            }
        }

        Destroy(this.gameObject);
    }
}
