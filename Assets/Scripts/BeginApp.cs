using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestApp : MonoBehaviour
{
    public GameEvent EnterLayout;
    public LayoutData layoutData;
    public RandomContext encRandomContext;

    void Start()
    {
        // Transition from lobby to layout
        layoutData.depth            = 9;
        layoutData.maxWidth         = 7;
        layoutData.randomSeed       = 65;
        layoutData.useSeed          = true;

        layoutData.InitializeStates();
        encRandomContext.ResetContext(layoutData.randomSeed == 0 ? 1 : layoutData.randomSeed);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
