using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestManager : MonoBehaviour
{
    public GameEvent EnterLayout;

    public PlayerInventory inventory;
    public int percentageHeal = 10;
    public CharacterData playerData;
    
    void Start()
    {
        float healamount = playerData.maxHealth * (1f / (float)percentageHeal);
        playerData.currentHealth += healamount;
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
