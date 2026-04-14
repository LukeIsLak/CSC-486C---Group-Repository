using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Cards/Haste")]
public class Haste : Cards
{
    private GameObject player;
    private PlayerController playerStats;
    private PlayerCharacter playerchar;

    public int buffTime = 10;
    public float attackspeedbuff;

    private GameObject hasteEffect;
    public Material hasteEffectMat;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        hasteEffect = GameObject.FindWithTag("HasteEffect");
        Image hasteEffectImg = hasteEffect.GetComponent<Image>();

        float timer = 0f;
        if (player == null || hasteEffect == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }

        playerStats = player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerchar = player.GetComponent(typeof(PlayerCharacter)) as PlayerCharacter;
        if (playerchar.hasteCount++ == 0) {
            hasteEffectImg.material.SetFloat("_speed", 0f);
            hasteEffectImg.material.SetFloat("_radius", 0.40f - playerchar.hasteCount*0.05f);
            hasteEffectImg.enabled = true;
        }
        playerStats.speed += 5f;
        playerchar.IncreaseAttackSpeed(attackspeedbuff);

        while (timer < buffTime) {
            hasteEffectImg.material.SetFloat("_speed", playerchar.movementSpeed + 0.25f);
            timer += Time.deltaTime;
            yield return null;
        }

        playerchar.DecreaseAttackSpeed(attackspeedbuff);
        playerStats.speed -= 5f;
        if (--playerchar.hasteCount == 0) {
            playerchar.movementSpeed = 0f;
            hasteEffectImg.material.SetFloat("_speed", 0f);
            hasteEffectImg.enabled = false;
        }
        else {
            hasteEffectImg.material.SetFloat("_radius", 0.40f - playerchar.hasteCount*0.05f);
        }
    }
}