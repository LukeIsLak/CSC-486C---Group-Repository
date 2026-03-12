using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/RunAwayBull")]
public class RunAwayBull : Cards
{
    private GameObject player;
    private PlayerCharacter playerChar;

    public int buffTime = 10;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }

        playerChar = player.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
        playerChar.runawayBullActive = true;
        yield return new WaitForSeconds(buffTime);
        playerChar.runawayBullActive = false;

    }
}
