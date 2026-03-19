using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestManager : MonoBehaviour
{
    public GameEvent EnterLayout;

    public PlayerInventory inventory;
    public float percentageHeal = 0.33f;
    public CharacterData playerData;
    
    private bool hasExited = false;
    void Start()
    {
        float healamount = playerData.maxHealth * percentageHeal;
        playerData.currentHealth += healamount;
        if (playerData.currentHealth > playerData.maxHealth) playerData.currentHealth = playerData.maxHealth;
        // refresh cards
        inventory.refreshCards();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (hasExited) return;
            hasExited = true;
            EnterLayout.Raise();
        }
    }
}
