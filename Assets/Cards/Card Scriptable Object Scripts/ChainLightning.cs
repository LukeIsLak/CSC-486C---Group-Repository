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

    private FMOD.Studio.EventInstance lightningSound;


    public override IEnumerator Play(Cards card)
    {
        //Change to a hitscan lighting bolt (same as lightning bolt code)
        player = GameObject.FindWithTag("Player");

        ChainLightningObject ChainLightning = Instantiate(ChainLightningPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation);
        PlayLightningSound();

        HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
        homing.Initialize();
        ChainLightningObject1 chainLightning = Instantiate(ChainLightningPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation).GetComponent<ChainLightningObject1>();
        chainLightning.Init(effectValue, range, 1, maxChain, new List<EnemyInterface>(), surfaceLayers);
        //HomingSystem homing = ChainLightning.GetComponent<HomingSystem>();
        //homing.Initialize();
        
        yield break;
    }

    private void PlayLightningSound() 
    {
        lightningSound = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/PlayerMagic/lightningHit");
        lightningSound.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(player));
        lightningSound.start();
        lightningSound.release();
    }
}