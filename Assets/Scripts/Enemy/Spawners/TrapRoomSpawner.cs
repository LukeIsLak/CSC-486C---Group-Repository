using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapRoomSpawner : MonoBehaviour
{
    public List<Wave> possibleWaves;
    public List<int> possibleWavesWeights;
    public List<Wave> waves;
    public List<Transform> spawnPoints;

    public int minWaves = 1;
    public int maxWaves = 5;

    public int totalWaveCount;

    public int currentWaveCount = 0;
    public int remainingEnemies;

    public GameEvent uponCompletion;

    public float? delay = 1f;

    private void OnValidate() {
        if (possibleWaves != null && possibleWavesWeights != null && possibleWaves.Count != possibleWavesWeights.Count) {
            Debug.LogError($"TrapRoomSpawner Initilization Error: possibleWaves.Count ({possibleWaves.Count}) does not match possibleWavesWeight.Count ({possibleWavesWeights.Count})", this);
        }

        foreach (int i in possibleWavesWeights) if (i <= 0) Debug.LogError($"TrapRoomSpawner Initilization Error: possibleWavesWeights has an instance of <= 0", this);
    }
    
    public void Initialize() {
        totalWaveCount = Random.Range(minWaves, maxWaves);
        waves = new List<Wave>();
        for (int i = 0; i < totalWaveCount; i++) {
            waves.Add(ChooseWave());
        }

        InstantiateWave(waves[currentWaveCount]);
    }

    public void InstantiateWave(Wave w) {
        float d = (delay != null) ? delay.Value : w.spawnDelay;
        remainingEnemies = w.enemyCounts.Sum();
        StartCoroutine(w.SpawnWaveDelay(spawnPoints, d, this));
    }

    public Wave ChooseWave() {
        float totalWeight = possibleWavesWeights.Sum();
        float r = UnityEngine.Random.Range(0, totalWeight);
        
        var weightedWaves = possibleWaves.Zip(possibleWavesWeights, (wave, weight) => new { wave, weight });
        float cumulative = 0f;
        foreach (var ww in weightedWaves) {
            cumulative += ww.weight;
            if (r < cumulative)
                return ww.wave;
        }
        return possibleWaves[0];
    }

    public void RemoveEnemy() { 
        remainingEnemies -= 1; 
        
        if (remainingEnemies <= 0) {
            currentWaveCount += 1;
            if (currentWaveCount + 1 >= totalWaveCount) {
                uponCompletion.Raise();
                return;
            }
            InstantiateWave(waves[currentWaveCount]);
        }
    }

    public void StartTrap(float trapDelay) {
        StartCoroutine(StartTrapSpawn(trapDelay));
    }

    private IEnumerator StartTrapSpawn(float trapDelay) {
        yield return new WaitForSeconds(trapDelay);
        Initialize();
    }
}
