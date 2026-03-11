using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestApp : MonoBehaviour
{
    public GameEvent EnterLayout;
    public LayoutData layoutData;
    public PlayerInventory inventory;
    public RandomContext encRandomContext;

    void Start()
    {
        // RANDOMNESS INITIALIZATION
        int runSeed                 = (int)System.DateTime.Now.Ticks;
        if (runSeed == 0)           runSeed = 1;
        layoutData.randomSeed       = runSeed;
        layoutData.useSeed          = true;
        layoutData.InitializeStates();
        encRandomContext.ResetContext(runSeed);

        // PLAYER INITIALIZATION
        inventory.Initialize();


        EnterLayout.Raise();

    }
}
