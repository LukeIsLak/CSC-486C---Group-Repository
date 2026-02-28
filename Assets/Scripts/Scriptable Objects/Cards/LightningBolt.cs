using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/LighteningBolt")]
public class LighteningBolt : Cards
{

    [SerializeField] private LightningObject lightningPrefab;

    private GameObject player;

    public override IEnumerator Play(Cards card)
    {
        player = GameObject.FindWithTag("Player");

        LightningObject lightning = Instantiate(daggerPrefab, player.transform.position + player.transform.forward * 2f, player.transform.rotation);

        HomingSystem homing = lightning.GetComponent<HomingSystem>();
        homing.Initialize();
        
        yield break;
    }
}