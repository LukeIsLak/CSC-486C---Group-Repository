using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapRoomSpawner : MonoBehaviour
{
    public List<Wave> possibleWaves;
    public List<int> possibleWavesWeights;
    public List<Wave> waves;
    public List<GameObject> spawnPoints;

    public int minWaves = 1;
    public int maxWaves = 5;

    public int totalWaveCount;

    public int currentWaveCount;
    public int remainingEnemies;

    private void OnValidate() {
        if (possibleWaves != null && possibleWavesWeights != null && possibleWaves.Count != possibleWavesWeights.Count) {
            Debug.LogError($"TrapRoomSpwaner Initilization Error: possibleWaves.Count ({possibleWaves.Count}) does not match possibleWavesWeight.Count ({possibleWavesWeights.Count})", this);
        }
    }
    
    public void Initialize() {
        totalWaveCount = Random.Range(minWaves, maxWaves);
        waves = new List<Wave>();
        for (int i = 0; i < totalWaveCount; i++) {
            waves.Add(ChooseWave());
        }
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

    public void RemoveEnemy() { remainingEnemies -= 1; }
}
