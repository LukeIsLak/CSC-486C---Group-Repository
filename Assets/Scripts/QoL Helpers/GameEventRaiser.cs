using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventRaiser : MonoBehaviour
{
    public GameEvent gameEvent;
    public bool onStart;

    void Start()
    {
        if (onStart) gameEvent.Raise();
    }
    
    public void Raise()
    {
        gameEvent.Raise();
    }
}
