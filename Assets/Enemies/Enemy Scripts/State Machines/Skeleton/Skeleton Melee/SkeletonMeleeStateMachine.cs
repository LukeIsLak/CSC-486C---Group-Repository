using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonMeleeStates {
    Spawn,
    SpawnWall,
    Idle,
    Wander,
    AgroApproach,
    DashAttack,
    SwingAttack
}

public class SkeletonMeleeStateMachine : StateMachine<SkeletonMelee, SkeletonMeleeStates>
{
    public void Awake() {
        /*Spawn*/

        /*SpawnWall*/

        /*Idle*/

        /*Wander*/

        /*AgroApproach*/
    }

    public override void CheckTransition(SkeletonMelee sm) {
        SkeletonMeleeStates currentState = sm.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(sm)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(sm);
                EnterUniversal(sm);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(sm);
                
                sm.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(SkeletonMelee sm) {
        var currentState = sm.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(sm);
        }
    }

    public override void EnterUniversal(SkeletonMelee sm) {

    }
}
