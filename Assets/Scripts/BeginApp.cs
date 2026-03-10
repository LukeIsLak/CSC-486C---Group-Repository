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
    private GameObject player;
    private Health playerhealth;

    void Start()
    {
        // RANDOMNESS INITIALIZATION
        layoutData.randomSeed       = (int)System.DateTime.Now.Ticks;
        layoutData.useSeed          = true;
        layoutData.InitializeStates();
        encRandomContext.ResetContext(layoutData.randomSeed == 0 ? 1 : layoutData.randomSeed);

        // PLAYER INITIALIZATION
        inventory.Initialize();
        // MAKE SURE PLAYER HAS FULL HEALTH
        //player = GameObject.FindWithTag("Player");
        //if (player == null) {
        //    Debug.Log("theres a problem");
        //}
        //playerhealth = player.GetComponent(typeof(Health)) as Health;
        //playerhealth.Heal(playerhealth.maxHealth);


        EnterLayout.Raise();

    }
}
