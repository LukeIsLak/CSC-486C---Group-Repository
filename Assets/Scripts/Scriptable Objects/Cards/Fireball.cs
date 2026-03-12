using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Fireball")]
public class Fireball : Cards
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;

    [SerializeField] private ShootFireball aoeprojprefab;
    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;


    [SerializeField]public float dmg = 20f;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        if (card is Fireball attackCard)
        {
            useAttackCard(attackCard);
            Debug.Log("Used card");
        }
        yield break;

    }

    public void useAttackCard(Fireball card)
    {
        //Switch case based on attacktype, will add more cases as more cards get developed
        handleAoE(card);
            
        
    }

    public void handleAoE(Fireball card)
    {
        //For AoE attacks, create the projectile and fire it forward based on the player position
        ShootFireball proj = Instantiate(aoeprojprefab, camera.transform.position + player.transform.forward * 2f, 
        Quaternion.identity);

        if (proj == null)
        {
            Debug.LogError("Projectile prefab not assigned!");
        }
        
        proj.Init(camera.transform.forward, card);
    }
}
