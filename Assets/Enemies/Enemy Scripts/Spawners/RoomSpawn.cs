using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawn : MonoBehaviour
{
    public List<Wave> possibleWaves;
    public List<int> possibleWavesWeights;
    public List<Wave> waves;
    public List<Transform> spawnPoints;

    public int minWaves = 1;
    public int maxWaves = 5;

    public bool hasWaves = false;

    public int totalWaveCount;

    public int currentWaveCount = 0;
    public int remainingEnemies;

    public GameEvent uponCompletion;

    [SerializeField] public float delay = 1f;

    private void OnValidate() {
        if (possibleWaves != null && possibleWavesWeights != null && possibleWaves.Count != possibleWavesWeights.Count) {
            Debug.LogError($"TrapRoomSpawner Initilization Error: possibleWaves.Count ({possibleWaves.Count}) does not match possibleWavesWeight.Count ({possibleWavesWeights.Count})", this);
        }

        foreach (int i in possibleWavesWeights) if (i <= 0) Debug.LogError($"TrapRoomSpawner Initilization Error: possibleWavesWeights has an instance of <= 0", this);
    }

    public void InstantiateWave(Wave w) {
        float d = (delay >= 0) ? delay : w.spawnDelay;
        if (hasWaves) remainingEnemies = w.enemyCounts.Sum();
        else remainingEnemies += w.enemyCounts.Sum();
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
        
        if (hasWaves) {
            if (remainingEnemies <= 0) {
                currentWaveCount += 1;
                if (currentWaveCount + 1 >= totalWaveCount) {
                    uponCompletion.Raise();
                    return;
                }
                InstantiateWave(waves[currentWaveCount]);
            }
        }
        else {
            if (remainingEnemies <= 0) uponCompletion.Raise();
        }
    }
}
