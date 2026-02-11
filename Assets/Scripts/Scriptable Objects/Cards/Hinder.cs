using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Hinder")]
public class Hinder : Cards
{
    private GameObject player;
    private DeckSystems deckSystem;

    public int effectTime = 10;
    public EnemyEffects enemyeffects;

    public GameEvent enemySpeedChange;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }
        //change there speed to be slower and use an event to let them know
        enemyeffects.changeEnemySpeed(0.5f);
        enemySpeedChange.Raise();

        //wait for the duration of the card
        yield return new WaitForSeconds(effectTime);

        //set everything back to normal and use an event to let the system know
        enemyeffects.resetEnemySpeed();
        enemySpeedChange.Raise();
    }
}
