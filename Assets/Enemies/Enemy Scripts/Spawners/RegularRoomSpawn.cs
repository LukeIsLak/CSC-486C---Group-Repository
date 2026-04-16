using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RegularRoomSpawner : RoomSpawn
{
    public void Awake() {
        StartCoroutine(Delay());
    }

    // XXX this is a hack please fix!!!
    private IEnumerator Delay() {
        yield return new WaitForSeconds(0.1f);
        totalWaveCount = Random.Range(minWaves, maxWaves);
        waves = new List<Wave>();
        for (int i = 0; i < totalWaveCount; i++) {
            waves.Add(ChooseWave());
        }

        if (hasWaves) InstantiateWave(waves[currentWaveCount], false);
        else for (int i = 0; i < totalWaveCount; i++) InstantiateWave(waves[i], false);
    }
}
