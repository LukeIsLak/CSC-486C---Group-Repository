using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//XXX combine this with the bat one
//XXX fill magic numbers
public struct VoidKnightWeightedAttacks {
    public VoidKnightAttacks attack;
    public float weight;
    public Func<bool> condition;
    public VoidKnightWeightedAttacks(VoidKnightAttacks attack, float weight, Func<bool> condition) {
        this.attack = attack;
        this.weight = weight;
        this.condition = condition;
    }
}

public enum VoidKnightAttacks {
    AttackCombo1,
    AttackFireBall,
    AttackDivineJudgement,
    AttackBehind
}

public enum VoidKnightStates {
    Spawn,

    Wander,                     // when wandering around the map
    AgroApproach,                       // when agroing towards the player
    Idle,                       // when idle

    AttackFireBall,             // fireball attack
    AttackDivineJudgement,      // divine judgement attack

    AttackDash,                 // dash attack

    AttackCombo1,
    AttackCombo1_pt1,           // combo attack
    AttackCombo1_pt2,
    AttackCombo1_pt3,

    AttackBehind                // attack from behind
}

public class VoidKnightStateMachine : StateMachine<VoidKnight, VoidKnightStates> {
    
    public void Awake() {
        /*Spawn*/
        AddTransition(VoidKnightStates.Spawn, VoidKnightStates.Idle, SpawnToIdle);
    
        /*Idle*/
        AddTransition(VoidKnightStates.Idle, VoidKnightStates.AgroApproach, IdleToAgroApproach);
    
        /*Agro Approach*/
        AddTransition(VoidKnightStates.AgroApproach, VoidKnightStates.Idle, AgroApproachToIdle);
        AddTransition(VoidKnightStates.AgroApproach, VoidKnightStates.AttackCombo1, AgroApproachToAttackCombo1);
        AddTransition(VoidKnightStates.AgroApproach, VoidKnightStates.AttackFireBall, AgroApproachToAttackFireBall);
        AddTransition(VoidKnightStates.AgroApproach, VoidKnightStates.AttackDivineJudgement, AgroApproachToAttackDivineJudgement);

        AddEnterState(VoidKnightStates.AgroApproach, EnterAgroApproachState);
        AddWhileState(VoidKnightStates.AgroApproach, WhileAgroApproachState);
    
        /*Attack Combo*/
        AddTransition(VoidKnightStates.AttackCombo1, VoidKnightStates.AttackCombo1_pt1, AttackCombo1Pt1);
        AddTransition(VoidKnightStates.AttackCombo1_pt1, VoidKnightStates.AttackCombo1_pt2, AttackCombo1Pt2);
        AddTransition(VoidKnightStates.AttackCombo1_pt2, VoidKnightStates.AttackCombo1_pt3, AttackCombo1Pt3);

        AddTransition(VoidKnightStates.AttackCombo1_pt3, VoidKnightStates.AgroApproach, AttackCombo1ToAgroApproach);
        AddTransition(VoidKnightStates.AttackCombo1_pt3, VoidKnightStates.AgroApproach, AttackCombo1ToIdle);

        AddEnterState(VoidKnightStates.AttackCombo1, EnterAttackCombo1);
        AddEnterState(VoidKnightStates.AttackCombo1_pt1, EnterAttackCombo1Pt1);
        AddEnterState(VoidKnightStates.AttackCombo1_pt2, EnterAttackCombo1Pt2);
        AddEnterState(VoidKnightStates.AttackCombo1_pt3, EnterAttackCombo1Pt3);

        AddWhileState(VoidKnightStates.AttackCombo1_pt1, WhileAttackComboPt1);
        AddWhileState(VoidKnightStates.AttackCombo1_pt2, WhileAttackComboPt2);
        AddWhileState(VoidKnightStates.AttackCombo1_pt3, WhileAttackComboPt3);

        /*Fireball Attack*/
        AddTransition(VoidKnightStates.AttackFireBall, VoidKnightStates.AgroApproach, AttackFireBallToAgroApproach);
        AddTransition(VoidKnightStates.AttackFireBall, VoidKnightStates.Idle, AttackFireBallToIdle);

        AddEnterState(VoidKnightStates.AttackFireBall, EnterAttackFireBall);

        /*Divine Judgement Attack*/
        AddTransition(VoidKnightStates.AttackDivineJudgement, VoidKnightStates.AgroApproach, AttackDivineJudgementToAgroApproach);
        AddTransition(VoidKnightStates.AttackDivineJudgement, VoidKnightStates.Idle, AttackDivineJudgementToIdle);

        AddEnterState(VoidKnightStates.AttackDivineJudgement, EnterAttackDivineJudgement);
    }

    public override void CheckTransition(VoidKnight v) {
        VoidKnightStates currentState = v.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(v)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(v);
                EnterUniversal(v);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(v);
                
                v.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(VoidKnight v) {
        var currentState = v.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(v);
        }
    }

    /******************************/
    /*   Transitions Conditions   */ 
    /******************************/

    /*From Spwan Transitions*/
    public bool SpawnToIdle(VoidKnight v) {
        return v.isInitialized;
    }

    /*From Idle Transitions*/
    public bool IdleToAgroApproach(VoidKnight v) {
        return Vector3.Distance(v.gameObject.transform.position, v.playerTransform.position) <= v.vkd.detectionRadius;
    }

    /*From Agro Approach Transitions*/
    public bool AgroApproachToIdle(VoidKnight v) {
        return Vector3.Distance(v.gameObject.transform.position, v.playerTransform.position) > v.vkd.detectionRadius;
    }

    public bool AgroApproachToAttackCombo1(VoidKnight v) {
        return v.canAttack && v.nextAttack != null && v.nextAttack.Value == VoidKnightAttacks.AttackCombo1;
    }

    public bool AgroApproachToAttackFireBall(VoidKnight v) {
        return v.canAttack && v.nextAttack != null && v.nextAttack.Value == VoidKnightAttacks.AttackFireBall;
    }

    public bool AgroApproachToAttackDivineJudgement(VoidKnight v) {
        return v.canAttack && v.nextAttack != null && v.nextAttack.Value == VoidKnightAttacks.AttackDivineJudgement;
    }

    /*From AttackCombo1*/
    public bool AttackCombo1Pt1(VoidKnight v) {
        return true;
    }

    public bool AttackCombo1Pt2(VoidKnight v) {
        return v.inAttackCombo1pt1 == false;
    }

    public bool AttackCombo1Pt3(VoidKnight v) {
        return v.inAttackCombo1pt2 == false;
    }

    public bool AttackCombo1ToAgroApproach(VoidKnight v) {
        return v.inAttackCombo1pt3 == false && v.isAttacking == false && Vector3.Distance(v.gameObject.transform.position, v.playerTransform.position) <= v.vkd.detectionRadius;
    }
    
    public bool AttackCombo1ToIdle(VoidKnight v) {
        return v.inAttackCombo1pt3 == false && v.isAttacking == false && Vector3.Distance(v.gameObject.transform.position, v.playerTransform.position) > v.vkd.detectionRadius;
    }

    /*From Fireball Attack*/
    public bool AttackFireBallToAgroApproach(VoidKnight v) {
        return v.isAttacking == false;
    }
    
    public bool AttackFireBallToIdle(VoidKnight v) {
        return v.isAttacking == false;
    }

    /*From Divine Judgement Attack*/
    public bool AttackDivineJudgementToAgroApproach(VoidKnight v) {
        return v.isAttacking == false;
    }
    
    public bool AttackDivineJudgementToIdle(VoidKnight v) {
        return v.isAttacking == false;
    }


    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public override void EnterUniversal(VoidKnight v) {
        // b.ChangeCurrentSpeed();
    }

    public void EnterAgroApproachState(VoidKnight v) {
        v.isAgro = true;
        v.checkPlayerPath = true;
        v.nextAttack = null;
        v.WaitToAttack();
    }

    public void EnterAttackCombo1(VoidKnight v) {

    }

    public void EnterAttackCombo1Pt1(VoidKnight v) {
        v.StartAttackCombo1(1);
    }

    public void EnterAttackCombo1Pt2(VoidKnight v) {
        v.StartAttackCombo1(2);
    }

    public void EnterAttackCombo1Pt3(VoidKnight v) {
        v.StartAttackCombo1(3);
    }

    public void EnterAttackFireBall(VoidKnight v) {
        v.CastFireBall();
    }

    public void EnterAttackDivineJudgement(VoidKnight v) {
        v.CastDivineJudgement();
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileAgroApproachState(VoidKnight v) {
        v.UpdateAgroMove();
    }

    public void WhileAttackComboPt1(VoidKnight v) {
        v.UpdateAttackCombo1(1);
    }

    public void WhileAttackComboPt2(VoidKnight v) {
        v.UpdateAttackCombo1(2);
    }

    public void WhileAttackComboPt3(VoidKnight v) {
        v.UpdateAttackCombo1(3);
    }

    /******************************/
    /*    Exit State Functions    */
    /******************************/
}
