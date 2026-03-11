using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/TidalWave")]
public class TidalWave : Cards
{
    // Definition from Design Team:
    // Summons a wave that pushes back all enemies it hits. Enemies accumulate damage while in the wave and upon hitting a surface.

    public WaveObject wavePrefab;

    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        Debug.Log("Used card");

        //Send wave away from the player
        Vector3 startPos = player.transform.position + player.transform.forward * 2f;

        WaveObject wave = Instantiate(wavePrefab, startPos, Quaternion.LookRotation(camera.transform.forward));

        wave.Init(player.transform.forward, card);

        yield break;

    }
}
