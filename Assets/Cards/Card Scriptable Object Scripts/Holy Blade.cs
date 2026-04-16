using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/HolyBlade")]
public class HolyBlade : Cards
{
    private GameObject player;
    private PlayerCharacter playerChar;

    public int buffTime = 10;
    public float increaseAmount = 0.2f; // expample 20% inrease would be entered as 0.2f

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
            increaseAmount = effectValue / 100;
        }

        playerChar = player.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
        playerChar.attackMultiplier += increaseAmount;
        yield return new WaitForSeconds(buffTime);
        playerChar.attackMultiplier -= increaseAmount;

    }
}
