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

        inventory.refreshCards();
        StartCoroutine(TryHeal());
    }


    public IEnumerator TryHeal()
    {
        float healamount = playerData.maxHealth * percentageHeal;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        while (!player)
        {
            yield return null;
            player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<Health>().Heal(healamount);
    }
}
