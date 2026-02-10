using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum BatStates {
    Perching,
    Perched,
    Flutter,
    PeckAttack,
    SwoopAttack,
    PeckCompleteRebound,
    PeckIncompleteRebound,
    Hit,
    Idle // returning to perched
}

public class BatStateManager : MonoBehaviour
{
    public Bat bat;
    //XXX maybe move this to a scriptable object??
    public Dictionary<BatStates, List<(BatStates toState, Func<Bat, bool> condition)>> conditionLookup = new Dictionary<BatStates, List<(BatStates, Func<Bat, bool>)>>();


    public void Awake() {
        // AddTransition(BatStates.Perched, BatStates.Flutter, );
        // AddTransition(BatStates.Flutter, BatStates.Attacking, FooB);
        // XXX hit should have 2 transition:
        //  one to be perched if not close to ceiling
        //  one to be fluttering if otherwise
        // AddTransition(BatStates.Hit, BatStates.Perched, FooC);
        // AddTransition(BatStates.Attacking, BatStates.Perched, FooD);
    }

    public void AddTransition(BatStates from, BatStates to, Func<Bat, bool> condition) {
        if (!conditionLookup.TryGetValue(from, out var list)) {
            list = new List<(BatStates, Func<Bat, bool>)>();
            conditionLookup[from] = list;
        }
        list.Add((to, condition));
    }
}
