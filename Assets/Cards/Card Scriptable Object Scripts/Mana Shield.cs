using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ManaShield")]
public class ManaShield : Cards
{
    private GameObject player;
    private Health playerhealth;

    public int shieldHits = 3;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }

        playerhealth = player.GetComponent(typeof(Health)) as Health;

        playerhealth.AddShield(shieldHits);

        yield return new WaitForSeconds(0.5f);
    }
}
