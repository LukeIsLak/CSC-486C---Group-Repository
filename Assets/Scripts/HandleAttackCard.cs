using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleAttackCard : MonoBehaviour
{
    [SerializeField] private HandleAoEProjectile aoeprojprefab;
    [SerializeField] private PlayerCharacter player;


    public void useAttackCard(AttackCards card){
        //Switch case based on attacktype, will add more cases as more cards get developed

        switch(card.attackType){
            case AttackType.AoE:
                handleAoE(card);
                break;
        }
    }

    public void handleAoE(AttackCards card){
        //For AoE attacks, create the projectile and fire it forward based on the player position
        HandleAoEProjectile proj = Instantiate(aoeprojprefab, player.transform.position, 
        Quaternion.identity);

        proj.Init(player.transform.forward, card);
    }
}
