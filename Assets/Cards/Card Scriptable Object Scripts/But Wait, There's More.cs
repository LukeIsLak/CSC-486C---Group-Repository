using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ButWaitThereMore")]
public class ButWaitTheresMore : Cards
{
    private GameObject player;
    private DeckSystems deckSystem;

    public int value = 2;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        if (player == null) {
            Debug.Log("theres a problem");
            yield return new WaitForSeconds(0.5f);
        }
        deckSystem = player.GetComponent(typeof(DeckSystems)) as DeckSystems;
        deckSystem.drawFromDiscard(value);
        yield return new WaitForSeconds(0.5f);
    }
}
