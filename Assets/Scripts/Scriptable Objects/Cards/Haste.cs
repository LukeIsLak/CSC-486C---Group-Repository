using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Haste")]
public class Haste : Cards
{
    private GameObject player;
    private PlayerController playerStats;

    public int buffTime = 10;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }

        playerStats = player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerStats.speed += 5f;
        yield return new WaitForSeconds(buffTime);
        playerStats.speed -= 5f;

    }


}