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
        AddTransition(RatStates.ColonyIdle, RatStates.AgroApproach, ColonyIdleToAgroApproach);

        AddEnterState(RatStates.ColonyIdle, EnterColonyIdleState);

        /*ColonyWander*/
        // XXX maybe add LonerWander
        AddTransition(RatStates.ColonyWander, RatStates.ColonyIdle, ColonyWanderToColonyIdle);
        AddTransition(RatStates.ColonyWander, RatStates.LonerWander, ColonyWanderToLonerWander);
        AddTransition(RatStates.ColonyWander, RatStates.AgroApproach, ColonyWanderToAgroApproach);

        AddEnterState(RatStates.ColonyWander, EnterColonyWanderState);
        AddWhileState(RatStates.ColonyWander, WhileColonyWanderState);

        /*LonerIdle*/
        AddTransition(RatStates.LonerIdle, RatStates.LonerWander, ColonyIdleToColonyWander);
        AddTransition(RatStates.LonerIdle, RatStates.ColonyIdle, ColonyIdleToLonerIdle);
        AddTransition(RatStates.LonerIdle, RatStates.AgroApproach, LonerIdleToAgroApproach);

        AddEnterState(RatStates.LonerIdle, EnterLonerIdleState);

        /*LonerWander*/
        AddTransition(RatStates.LonerWander, RatStates.LonerIdle, LonerWanderToLonerIdle);
        AddTransition(RatStates.LonerWander, RatStates.ColonyWander, LonerWanderToColonyWander);
        AddTransition(RatStates.LonerWander, RatStates.AgroApproach, LonerWanderToAgroApproach);

        AddEnterState(RatStates.LonerWander, EnterLonerWanderState);
        AddWhileState(RatStates.LonerWander, WhileLonerWanderState);

        /*AgroApproach*/
        // XXX do we want the colonywander here?
        AddTransition(RatStates.AgroApproach, RatStates.ColonyWander, AgroApproachToColonyWander);
        AddTransition(RatStates.AgroApproach, RatStates.ColonyIdle, AgroApproachToColonyIdle);
        // AddTransition(RatStates.AgroApproach, RatStates.LonerWander, AgroApproachToLonerWander);
        AddTransition(RatStates.AgroApproach, RatStates.LonerIdle, AgroApproachToLonerIdle);
        AddTransition(RatStates.AgroApproach, RatStates.Leap, AgroApproachToLeap);

        AddEnterState(RatStates.AgroApproach, EnterAgroApproachState);
        AddWhileState(RatStates.AgroApproach, WhileAgroApproachState);
        AddExitState(RatStates.AgroApproach, ExitAgroApproachState);

        /*Leap*/
        AddTransition(RatStates.Leap, RatStates.AgroApproach, LeapToAgroApproach);
        AddTransition(RatStates.Leap, RatStates.ColonyWander, LeapToColonyWander);
        AddTransition(RatStates.Leap, RatStates.ColonyIdle, LeapToColonyIdle);
        // AddTransition(RatStates.Leap, RatStates.LonerWander, AgroApproachToLonerWander);
        AddTransition(RatStates.Leap, RatStates.LonerIdle, LeapToLonerIdle);

        AddEnterState(RatStates.Leap, EnterLeapState);
        AddExitState(RatStates.Leap, ExitLeapState);

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
        return r.isInitialized && !r.isLoner;
    }

    public bool SpawnToLonerIdle(Rat r) {
        return r.isInitialized && r.isLoner;
    }

    /*From ColonyIdle Transitions*/
    public bool ColonyIdleToColonyWander(Rat r) {
        return r.canWander;
    }

    public bool ColonyIdleToLonerIdle(Rat r) {
        return r.isLoner;
    }

    public bool ColonyIdleToAgroApproach(Rat r) {
        return Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.detectionRadius;
    }

    /*From ColonyWandering Transitions*/
    public bool ColonyWanderToColonyIdle(Rat r) {
        return r.doneWandering;
    }

    public bool ColonyWanderToLonerWander(Rat r) {
        return r.isLoner && r.isWandering;
    }

    public bool ColonyWanderToAgroApproach(Rat r) {
        return Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.detectionRadius;
    }

    /*From LonerIdle Transitions*/
    public bool LonerIdleToLonerWander(Rat r) {
        return r.canWander;
    }

    public bool LonerIdleToColonyIdle(Rat r) {
        return !r.isLoner;
    }

    public bool LonerIdleToAgroApproach(Rat r) {
        return Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.detectionRadius;
    }


    /*From LonerWadnering Transitions*/
    public bool LonerWanderToLonerIdle(Rat r) {
        return r.doneWandering;
    }

    public bool LonerWanderToColonyWander(Rat r) {
        return !r.isLoner && r.isWandering;
    }

    public bool LonerWanderToAgroApproach(Rat r) {
        return Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.detectionRadius;
    }

    /*From AgroApproach Transitions*/
    public bool AgroApproachToColonyWander(Rat r) {
        return     !r.isLoner 
                && r.isWandering
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
    }

    public bool AgroApproachToColonyIdle(Rat r) {
        return     !r.isLoner 
                && !r.isWandering
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
    }

    public bool AgroApproachToLonerIdle(Rat r) {
        return     r.isLoner
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
    }

    public bool AgroApproachToLeap(Rat r) {
        return     r.canLeap 
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.leapRadius;
    }

    /*From Leap Transitions*/
    public bool LeapToAgroApproach(Rat r) {
        return     r.doneLeap
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) <= r.rd.detectionRadius;
    }

    public bool LeapToColonyWander(Rat r) {
        return     r.doneLeap
                && !r.isLoner 
                && r.isWandering
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
    }

    public bool LeapToColonyIdle(Rat r) {
        return     r.doneLeap
                && !r.isLoner 
                && !r.isWandering
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
    }

    public bool LeapToLonerIdle(Rat r) {
        return     r.doneLeap
                && r.isLoner
                && Vector3.Distance(r.gameObject.transform.position, r.playerTransform.position) > r.rd.detectionRadius;
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
        if (!r.isMoving) r.StartWanderStagger();
    }

    public void EnterAgroApproachState(Rat r) {
        r.isAgro = true;
    }

    public void EnterLeapState(Rat r) {
        r.StartLeap();
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileColonyWanderState(Rat r) {
        if (r.isMoving) r.UpdateColonyMove();
    }

    public void WhileLonerWanderState(Rat r) {
        if (r.isMoving) r.UpdateLonerMove();
    }

    public void WhileAgroApproachState(Rat r) {
        r.UpdateAgroApproach();
    }

    /******************************/
    /*    Exit State Functions    */
    /******************************/

    public void ExitAgroApproachState(Rat r) {
        r.isAgro = false;
    }

    public void ExitLeapState(Rat r) {
        r.StartLeapCooldown();
    }

    public override void EnterUniversal(Rat r) {}
}
