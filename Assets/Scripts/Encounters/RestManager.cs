using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestManager : MonoBehaviour
{
    public GameEvent EnterLayout;
    
    void Start()
    {
        // Do reststuffs   
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
