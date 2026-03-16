using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapRoomSpawner : RoomSpawn
{
    public void Initialize() {
        totalWaveCount = Random.Range(minWaves, maxWaves);
        waves = new List<Wave>();
        for (int i = 0; i < totalWaveCount; i++) {
            waves.Add(ChooseWave());
        }

        InstantiateWave(waves[currentWaveCount]);
    }

    public void StartTrap(float trapDelay) {
        StartCoroutine(StartTrapSpawn(trapDelay));
    }

    private IEnumerator StartTrapSpawn(float trapDelay) {
        yield return new WaitForSeconds(trapDelay);
        Initialize();
    }
}
