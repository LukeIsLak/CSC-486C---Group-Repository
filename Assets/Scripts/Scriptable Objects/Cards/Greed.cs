using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Greed")]
public class Greed : Cards
{
    private GameObject player;
    private DeckSystems deckSystem;
    public int amount;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }
        deckSystem = player.GetComponent(typeof(DeckSystems)) as DeckSystems;
        deckSystem.GreedDrawCards(amount);
        yield return new WaitForSeconds(0.5f);
    }
}
