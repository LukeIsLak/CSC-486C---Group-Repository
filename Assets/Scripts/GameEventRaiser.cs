using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventRaiser : MonoBehaviour
{
    public GameEvent gameEvent;
    public void Raise()
    {
        gameEvent.Raise();
    }
}
