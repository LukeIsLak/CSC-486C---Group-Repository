using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RatStates : StateMachine<Ratm RatStates> {
    Spawn,
    Idle,
    Wander,
    AgroApproach,
    Leap,
    RunAway,
    Hit
}

public class RatStateManager : MonoBehaviour
{
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

    public override void CheckUpdate(Rat b) {
        var currentState = r.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(b);
        }
    }
}
