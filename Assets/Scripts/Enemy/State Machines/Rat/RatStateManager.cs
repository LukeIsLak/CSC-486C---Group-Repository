using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RatStates {
    Spawn,
    ColonyIdle,
    ColonyWander,
    LonerIdle,
    LonerWander,
    AgroApproach,
    Leap,
    RunAway,
    Hit
}

public class RatStateManager : StateMachine<Rat, RatStates>
{
    public RatColonyManager rcm;
    public void Awake() {
        /*Spawn*/
        AddTransition(RatStates.Spawn, RatStates.ColonyIdle, SpawnToColonyIdle);
        AddTransition(RatStates.Spawn, RatStates.LonerIdle, SpawnToLonerIdle);

        /*ColonyIdle*/
        AddTransition(RatStates.ColonyIdle, RatStates.ColonyWander, ColonyIdleToColonyWander);
        AddTransition(RatStates.ColonyIdle, RatStates.LonerIdle, ColonyIdleToLonerIdle);

        AddEnterState(RatStates.ColonyIdle, EnterColonyIdleState);

        /*ColonyWander*/
        AddTransition(RatStates.ColonyWander, RatStates.ColonyIdle, ColonyWanderToColonyIdle);

        AddEnterState(RatStates.ColonyWander, EnterColonyWanderState);
        AddWhileState(RatStates.ColonyWander, WhileColonyWanderState);

        /*LonerIdle*/
        AddTransition(RatStates.LonerIdle, RatStates.LonerWander, ColonyIdleToColonyWander);
        AddTransition(RatStates.LonerIdle, RatStates.ColonyIdle, ColonyIdleToLonerIdle);

        AddEnterState(RatStates.LonerIdle, EnterLonerIdleState);

        /*LonerWander*/
        AddTransition(RatStates.LonerWander, RatStates.LonerIdle, LonerWanderToLonerIdle);

        /*AgroApproach*/

        /*Leap*/

        /*RunAway*/
    }

    public override void CheckTransition(Rat r) {
        RatStates currentState = r.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(r)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(r);
                EnterUniversal(r);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(r);
                
                r.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(Rat r) {
        var currentState = r.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(r);
        }
    }

    /******************************/
    /*   Transitions Conditions   */ 
    /******************************/

    /*From Spawn Transitions*/
    public bool SpawnToColonyIdle(Rat r) {
        return !r.isLoner;
    }

    public bool SpawnToLonerIdle(Rat r) {
        return r.isLoner;
    }

    /*From ColonyIdle Transitions*/
    public bool ColonyIdleToColonyWander(Rat r) {
        return r.canWander;
    }

    public bool ColonyIdleToLonerIdle(Rat r) {
        return r.isLoner;
    }

    /*From ColonyWandering Transitions*/
    public bool ColonyWanderToColonyIdle(Rat r) {
        return r.doneWandering;
    }

    /*From LonerIdle Transitions*/
    public bool LonerIdleToLonerWander(Rat r) {
        return r.canWander;
    }

    public bool LonerIdleToColonyIdle(Rat r) {
        return !r.isLoner;
    }


    /*From LonerWadnering Transitions*/
    public bool LonerWanderToLonerIdle(Rat r) {
        return r.doneWandering;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public void EnterColonyIdleState(Rat r) {
        r.doneWandering = false;
        if (!r.isIdle) r.StartIdleDuration();
    }

    public void EnterColonyWanderState(Rat r) {
        if (!r.isMoving) r.StartWanderStagger();
    }

    public void EnterLonerIdleState(Rat r) {
        if (!r.isIdle) r.StartIdleDuration();
    }

    public void EnterLonerWanderState(Rat r) {
        
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileColonyWanderState(Rat r) {
        if (r.isMoving) r.UpdateColonyMove();
    }

    /******************************/
    /*    Exit State Functions    */
    /******************************/

    public override void EnterUniversal(Rat r) {}
}
