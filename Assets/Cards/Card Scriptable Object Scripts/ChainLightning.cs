using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ChainLightning")]
public class ChainLightning : Cards
{

    [SerializeField] private ChainLightningObject ChainLightningPrefab;

    private GameObject player;

    public override IEnumerator Play(Cards card)
    {
        //Change to a hitscan lighting bolt (same as lightning bolt code)
        player = GameObject.FindWithTag("Player");

        ChainLightningObject ChainLightning = Instantiate(ChainLightningPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation);

        HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
        homing.Initialize();
        
        yield break;
    }
}