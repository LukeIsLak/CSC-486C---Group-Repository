using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Haste")]
public class Haste : Cards
{
    private GameObject player;
    private PlayerController playerStats;
    private PlayerCharacter playerchar;

    public int buffTime = 10;
    public float attackspeedbuff;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }

        playerStats = player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerchar = player.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
        playerStats.speed += 5f;
        playerchar.IncreaseAttackSpeed(attackspeedbuff);
        yield return new WaitForSeconds(buffTime);
        playerchar.DecreaseAttackSpeed(attackspeedbuff);
        playerStats.speed -= 5f;

    }


}