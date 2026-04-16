using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/TidalWave")]
public class TidalWave : Cards
{
    // Definition from Design Team:
    // Summons a wave that pushes back all enemies it hits. Enemies accumulate damage while in the wave and upon hitting a surface.

    public WaveObject wavePrefab;
    public int N = 5;
    public float alpha = 60f;

    private GameObject player; // TODO : LK - I left this in for now, in case you guys want it
    private GameObject camera;

    public override IEnumerator Play(Cards card){
        player = GameObject.FindWithTag("Player");
        camera = GameObject.FindWithTag("MainCamera");
        Debug.Log("Used card");

        //Send wave away from the player
        float startAngle = -alpha / 2f;
        float angleStep = (N == 1) ? 0 : alpha / (N - 1);

        Vector3 startPos = player.transform.position + player.transform.forward * 1f;
        Vector3 forward = player.transform.forward;

        for (int i = 0; i < N; i++)
        {
            float angle = startAngle + angleStep * i;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up) * Quaternion.LookRotation(forward);
            Vector3 dir = rotation * Vector3.forward;

            WaveObject wave = Instantiate(wavePrefab, startPos, Quaternion.LookRotation(dir));
            wave.Init(dir, card);
        }

        yield break;

    }
}
