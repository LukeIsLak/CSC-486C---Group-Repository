using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleObjective : MonoBehaviour
{
    public GameEvent EnterLayout;
    public LayoutData layoutData;

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "Player")
        {
            EnterLayout.Raise();
        }
    }
}
