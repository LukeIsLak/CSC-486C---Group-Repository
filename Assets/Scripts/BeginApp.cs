using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestApp : MonoBehaviour
{
    public GameEvent EnterLayout;
    public LayoutData layoutData;

    void Start()
    {
            // Transition from lobby to layout
            layoutData.depth            = 9;
            layoutData.maxWidth         = 7;
            layoutData.randomSeed       = 0;
            layoutData.useSeed          = true;
            layoutData.shouldGenerate   = true;
            layoutData.completedIndices.Clear();
            layoutData.completedIndices.Add(0);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
