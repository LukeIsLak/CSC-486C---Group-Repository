using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Fireball")]
public class Fireball : Cards
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;

    [SerializeField] private ShootFireball aoeprojprefab;
    private GameObject player;


    [SerializeField]public float dmg = 20f;
    private Vector3 dir; 

    [SerializeField] private LayerMask enemylayer;


    public override void Play(Cards card){
    if (card is Fireball attackCard)
    {
        useAttackCard(attackCard);
        Debug.Log("Used card");
    }

    }

    void Start(){
        player = GameObject.FindWithTag("Player");
        if (player == null) return;
    }

    public void useAttackCard(Fireball card)
    {
        //Switch case based on attacktype, will add more cases as more cards get developed
        handleAoE(card);
            
        
    }

    public void handleAoE(Fireball card)
    {
        //For AoE attacks, create the projectile and fire it forward based on the player position
        ShootFireball proj = Instantiate(aoeprojprefab, player.transform.position, 
        Quaternion.identity);

        proj.Init(player.transform.forward, card);
    }
}
