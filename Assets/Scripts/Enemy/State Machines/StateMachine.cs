using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class StateMachine<TEntity, TState> : MonoBehaviour
{
    public List<TEntity> entities;

    public Dictionary<TState, List<(TState toState, Func<TEntity, bool> condition)>> conditionLookup 
     = new Dictionary<TState, List<(TState, Func<TEntity, bool>)>>();

    public Dictionary<TState, Action<TEntity>> enterStates 
     = new Dictionary<TState, Action<TEntity>>();
    public Dictionary<TState, Action<TEntity>> whileStates 
     = new Dictionary<TState, Action<TEntity>>();
    public Dictionary<TState, Action<TEntity>> exitStates 
     = new Dictionary<TState, Action<TEntity>>();

    public void Update() {
        foreach (TEntity entity in entities) {
            CheckTransition(entity);
            CheckUpdate(entity);
        }
    }

    public void AddTransition(TState from, TState to, Func<TEntity, bool> condition) {
        if (!conditionLookup.TryGetValue(from, out var list)) {
            list = new List<(TState, Func<TEntity, bool>)>();
            conditionLookup[from] = list;
        }
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

    public abstract void CheckTransition(TEntity entity);
    public abstract void CheckUpdate(TEntity entity);

    //XXX do I even want this here?
    public abstract void EnterUniversal(TEntity entity);
}
