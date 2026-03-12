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
    public GameObject player;
    private Health playerhealth;
    public CharacterData characterData;

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
        // MAKE SURE PLAYER HAS FULL HEALTH
        if (player == null) {
            Debug.Log("theres a problem");
        } else {
            playerhealth = player.GetComponent(typeof(Health)) as Health;
            playerhealth.Init(characterData.maxHealth, characterData.currentHealth);
        }


        EnterLayout.Raise();

    }
}
