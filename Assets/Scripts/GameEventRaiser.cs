using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventRaiser : MonoBehaviour
{
    public GameEvent gameEvent;
    public Animator animator;
    public bool onStart;

    void Start()
    {
        if (onStart) gameEvent.Raise();
    }
    
    public void Raise()
    {
        animator.CrossFadeInFixedTime("Pull", 0.2f);
        gameEvent.Raise();
    }
}
