using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Recall")]
public class Recall : Cards
{
    private GameObject player;
    private DeckSystems deckSystem;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }
        deckSystem = player.GetComponent(typeof(DeckSystems)) as DeckSystems;
        deckSystem.recallCard();
        yield return new WaitForSeconds(0.5f);
    }
}
