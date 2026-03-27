using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestApp : MonoBehaviour
{
    public GameEvent EnterLayout;
    public LayoutData layoutData;
    public PlayerInventory inventory;
    [SerializeField]
    private RandomContext encRandomContext;
    public CharacterData characterData;

    public bool started;

    void Start()
    {
        started = false;
        // RANDOMNESS INITIALIZATION
        int runSeed                 = (int)System.DateTime.Now.Ticks;
        if ((uint)runSeed == 0)     {Debug.Log("Seed 0"); runSeed = 1;}
        layoutData.randomSeed       = runSeed;
        layoutData.useSeed          = true;
        layoutData.InitializeStates();
        encRandomContext.ResetContext((uint)runSeed);

        // MAKE SURE PLAYER HAS FULL HEALTH
        characterData.currentHealth = characterData.maxHealth;

    }
    public void Begin()
    {
        if (started) return;
        started = true;
        inventory.Initialize();
        EnterLayout.Raise();
    }
}
