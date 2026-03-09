using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// combine this with the bat one
public struct VoidKnightAttacks {
    public VoidKnightStates attack;
    public float weight;
    public VoidKnightAttacks(VoidKnightStates attack, float weight) {
        this.attack = attack;
        this.weight = weight;
    }
}

public enum VoidKnightStates {
    Spawn,

    Wander,                     // when wandering around the map
    AgroApproach,                       // when agroing towards the player
    Idle,                       // when idle

    AttackFireBall,             // fireball attack
    AttackDivineJudgement,      // divine judgement attack

    AttackDash,                 // dash attack

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

        AddEnterState(VoidKnightStates.AgroApproach, EnterAgroApproachState);
        AddWhileState(VoidKnightStates.AgroApproach, WhileAgroApproachState);
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

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public override void EnterUniversal(VoidKnight v) {
        // b.ChangeCurrentSpeed();
    }

    public void EnterAgroApproachState(VoidKnight v) {
        v.isAgro = true;
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileAgroApproachState(VoidKnight v) {
        v.UpdateAgroMove();
    }

    /******************************/
    /*    Exit State Functions    */
    /******************************/
}
