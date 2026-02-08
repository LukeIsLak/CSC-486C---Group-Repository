using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners =
        new List<GameEventListener>();

    public void Raise()
    {
        for(int i = listeners.Count - 1; i >=0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }
    public void AddListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener)) listeners.Add(listener);
    }
    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
