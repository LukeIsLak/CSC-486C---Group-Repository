using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestManager : MonoBehaviour
{
    public GameEvent EnterLayout;

    private GameObject player;
    private Health playerhealth;
    public PlayerInventory inventory;
    public int percentageHeal = 10;
    
    void Start()
    {
        // get references
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
        }
        playerhealth = player.GetComponent(typeof(Health)) as Health;
        //heal the player
        float healamount = playerhealth.maxHealth * (1f / (float)percentageHeal);
        playerhealth.Heal(healamount);
        // refresh cards
        inventory.refreshCards();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
