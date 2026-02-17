using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StateMachine<TEntity, TState> : MonoBehaviour
{
    public List<TEntity> entities;

    /*
        Transition lookup table is a dictionary which contains they following keys / values:
            - (key)     state starting from
            - (value)   lists of the following tuples / pairs, where each pair contains
                            - state attempting to go to
                            - boolean function to validate this transition is 
                            possible (takes entity)

        Idea here is that for a given state, a manager can lookup possible traversal routes.

        NOTE: Order for the states matter here! If a transition takes priority, it must come
        earlier in the list.
    */
    public Dictionary<TState, List<(TState toState, Func<TEntity, bool> condition)>> conditionLookup 
     = new Dictionary<TState, List<(TState, Func<TEntity, bool>)>>();

    /*

        The state dictionaries (enterStates, whileStates, exitStates) are designed so that the
        logic for handling an entity’s behavior in a given state is managed by the state 
        machine itself—not by the entity. This means that, for any entity, the state machine 
        is responsible for checking conditions, running state-specific actions, and handling 
        transitions. 

        State lookup table is a dictionary which contains they following keys / values:
            - (key)     current state
            - (value)   action (function) to do if in the current state
        
        The entity doesn’t need to know how to check its own state or manage transitions; 
        instead, the state machine looks up the relevant actions and conditions for each 
        state and applies them to the entity as needed.

        NOTE: Entities should have separate functions which can combines theses. However,
        entities should remain stateless themselves (to some degree).

        In short: the states encapsulate all the logic for what happens to an entity, 
        so the entity itself stays simple and unaware of the state management details.
    */
    public Dictionary<TState, Action<TEntity>> enterStates 
     = new Dictionary<TState, Action<TEntity>>();
    public Dictionary<TState, Action<TEntity>> whileStates 
     = new Dictionary<TState, Action<TEntity>>();
    public Dictionary<TState, Action<TEntity>> exitStates 
     = new Dictionary<TState, Action<TEntity>>();

    public void Update() {
        /*
            Foreach stored entity:
                check if a transition exists, transition to it if possible
                update the entity given the current state
        */
        foreach (TEntity entity in entities) {
            CheckTransition(entity);
            CheckUpdate(entity);
        }
    }

    public void AddTransition(TState from, TState to, Func<TEntity, bool> condition) {
        /*If no list currently exists for a state, create one*/
        if (!conditionLookup.TryGetValue(from, out var list)) {
            list = new List<(TState, Func<TEntity, bool>)>();
            conditionLookup[from] = list;
        }

        /*Add transition to list*/
        list.Add((to, condition));
    }
    

    public void AddEnterState(TState state, Action<TEntity> f) {
        enterStates[state] = f;
    }
    public void AddWhileState(TState state, Action<TEntity> f) {
        whileStates[state] = f;
    }
    public void AddExitState(TState state, Action<TEntity> f) {
        exitStates[state] = f;
    }

    /*Explicit function which any state machine requires, but may handle differently*/
    public abstract void CheckTransition(TEntity entity);
    public abstract void CheckUpdate(TEntity entity);

    //XXX do I even want this here?
    public abstract void EnterUniversal(TEntity entity);
}