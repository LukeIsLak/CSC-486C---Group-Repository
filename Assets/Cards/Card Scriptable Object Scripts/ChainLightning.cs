using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ChainLightning")]
public class ChainLightning : Cards
{

    [SerializeField] private GameObject ChainLightningPrefab;
    [SerializeField] private float range;
    [SerializeField] private int maxChain;
    [SerializeField] private LayerMask surfaceLayers;
    private GameObject player;

    public override IEnumerator Play(Cards card)
    {
        //Change to a hitscan lighting bolt (same as lightning bolt code)
        player = GameObject.FindWithTag("Player");

        ChainLightningObject1 chainLightning = Instantiate(ChainLightningPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation).GetComponent<ChainLightningObject1>();
        chainLightning.Init(effectValue, range, 1, maxChain, new List<EnemyInterface>(), surfaceLayers);
        //HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
        //homing.Initialize();
        
        yield break;
    }
}