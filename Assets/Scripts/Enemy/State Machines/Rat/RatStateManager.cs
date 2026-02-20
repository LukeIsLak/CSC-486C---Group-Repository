using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RatStates {
    Spawn,
    ColonyIdle,
    ConolyWander,
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

        /*Idle*/

        /*Wander*/

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
    public bool SpawnToIdle(Rat r) {
        return true;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    /******************************/
    /*   While State Functions    */
    /******************************/

    /******************************/
    /*    Exit State Functions    */
    /******************************/

    public override void EnterUniversal(Rat r) {}
}
