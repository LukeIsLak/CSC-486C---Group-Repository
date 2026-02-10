using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/AttackCards")]
public class AttackCards : Cards
{
    public int attackDamage;
    public string element;
    public AttackType attackType;

    [SerializeField] HandleAttackCard attackHandler;

    public override void Play(Cards card){
    if (card is AttackCards attackCard)
    {
        attackHandler.useAttackCard(attackCard);
    }

    }
}

public enum AttackType{
    AoE
}
