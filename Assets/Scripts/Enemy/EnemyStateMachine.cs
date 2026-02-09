using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class StateTransitionEvent : UnityEvent<GameObject>{}

[CreateAssetMenu(menuName = "Enemy/State")]
public class EnemyStateMachine : ScriptableObject
{
    public string stateName;

    [Header("Transitions")]
    [SerializeField]
    private List<EnemyStateMachine> transitionStates = new();

    [SerializeField]
    private List<StateTransitionEvent> transitionConditions = new();

    [SerializeField]
    private List<bool> transitionAvailability = new();

    [Header("Events")]
    [SerializeField]
    public UnityEvent onEnterStateEvent = new UnityEvent();

    // Helper methods so MonoBehaviours can subscribe to ScriptableObject events at runtime.
    public void InvokeOnEnter() {
        onEnterStateEvent?.Invoke();
    }

    public void AddOnEnterListener(UnityEngine.Events.UnityAction action) {
        if (onEnterStateEvent == null) onEnterStateEvent = new UnityEvent();
        onEnterStateEvent.AddListener(action);
    }

    public void RemoveOnEnterListener(UnityEngine.Events.UnityAction action) {
        onEnterStateEvent?.RemoveListener(action);
    }

    // Added: runtime flag to indicate this state is currently able to transition.
    [NonSerialized]
    public bool ableToTransition = false;
}
