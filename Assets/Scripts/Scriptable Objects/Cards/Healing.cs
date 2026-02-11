using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Healing")]
public class Healing : Cards
{
    private GameObject player;
    private Health playerhealth;

    public int percentage = 10;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }
        playerhealth = player.GetComponent(typeof(Health)) as Health;
        float healamount = playerhealth.maxHealth * (1f / (float)percentage);
        Debug.Log(playerhealth.maxHealth * (1f / (float)percentage));
        playerhealth.Heal(healamount);
        yield return new WaitForSeconds(0.5f);
    }
}
