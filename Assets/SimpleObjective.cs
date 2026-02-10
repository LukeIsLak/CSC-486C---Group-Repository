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
            // Transition from lobby to layout
            layoutData.depth            = 9;
            layoutData.maxWidth         = 7;
            layoutData.randomSeed       = (int)System.DateTime.Now.Ticks;
            layoutData.useSeed          = true;
            layoutData.shouldGenerate   = true;
            layoutData.completedIndices.Clear();
            layoutData.completedIndices.Add(0);
            layoutData.layerDistance    = 4;
            layoutData.encounterSep     = 4;
            EnterLayout.Raise();
        }
    }
}
