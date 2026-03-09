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
        // RANDOMNESS INITIALIZATION
        layoutData.randomSeed       = (int)System.DateTime.Now.Ticks;
        layoutData.useSeed          = true;
        layoutData.InitializeStates();
        encRandomContext.ResetContext(layoutData.randomSeed == 0 ? 1 : layoutData.randomSeed);

        // PLAYER INITIALIZATION


        EnterLayout.Raise();

    }
}
