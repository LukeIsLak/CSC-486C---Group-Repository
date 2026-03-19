using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapRoomSpawner : RoomSpawn, ITrapSequence
{
    public float trapDelay = 1.5f;
    public void Initialize() {
        totalWaveCount = Random.Range(minWaves, maxWaves);
        waves = new List<Wave>();
        for (int i = 0; i < totalWaveCount; i++) {
            waves.Add(ChooseWave());
        }

        InstantiateWave(waves[currentWaveCount]);
    }

    public void Begin() {
        StartCoroutine(StartTrapSpawn(trapDelay));
    }

    private IEnumerator StartTrapSpawn(float trapDelay) {
        yield return new WaitForSeconds(trapDelay);
        Initialize();
    }
}
