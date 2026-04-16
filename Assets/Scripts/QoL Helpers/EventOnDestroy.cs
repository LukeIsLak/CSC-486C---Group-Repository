using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOnDestroy : MonoBehaviour
{
    public GameEvent RaiseOnDestroy;

    public void OnDestroy()
    {
        RaiseOnDestroy.Raise();        
    }
}
