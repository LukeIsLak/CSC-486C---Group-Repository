using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyType {
    Bat,
    Rat
}

[CreateAssetMenu(menuName="Data/Wave")]
public class Wave : ScriptableObject
{
    public List<GameObject> enemyPrefabs;
    public List<EnemyType> enemyTypes;
    public List<int> enemyCounts;
    public float spawnDelay;
    public bool orderedSpawn = true;
    
    private void OnValidate() {
        if (enemyPrefabs != null && enemyCounts != null && enemyPrefabs.Count != enemyCounts.Count) {
            Debug.LogError($"Wave ScriptableObject: enemyPrefabs.Count ({enemyPrefabs.Count}) does not match enemySpawns.Count ({enemyCounts.Count})", this);
        }
        foreach (int i in enemyCounts) if (i <= 0) Debug.LogError($"Wave ScriptableObject: enemyPrefabs.Count has an instance of <= 0", this);
    }


    public IEnumerator SpawnWaveDelay(List<Transform> spawnPoints, float delay, RoomSpawn rs) {
        if (enemyPrefabs == null || enemyCounts == null) yield break;
        int count = enemyCounts.Sum();
        Debug.Log(count);

        List<int> ec = null;
        int c = 0;
        List<bool> available = null;


        if (!orderedSpawn) {
            ec = enemyCounts;
            c = enemyCounts.Count;
            available = Enumerable.Repeat(true, c).ToList();
        }

        for (int i = 0; i < count; i++) {
            int index;
            int sp = Random.Range(0, spawnPoints.Count);
            if (orderedSpawn) {
                int sum = 0;
                index = enemyCounts.TakeWhile(n => { 
                                                if (sum + n >= i) return false; 
                                                sum += n;
                                                return true;}).Count();
            }
            else {
                int _index = Random.Range(0, ec.Count);
                int seen = 0;
                index = available.TakeWhile(x => {
                                                if (x) seen++;
                                                return seen <= _index; }).Count() - 1;
                if (--ec[index] == 0) {
                    available[index] = false;
                    c -= 1;
                }
            }

            int enemyTypeIndex = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyPrefab = enemyPrefabs[index];
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, spawnPoints[sp].position, Quaternion.identity);
            newEnemy.GetComponent<EnemyInterface>().rs = rs;

            switch (enemyTypes[index]) {
                case EnemyType.Rat:
                    newEnemy.GetComponent<Rat>().initialize_nma();
                    break;
                case EnemyType.Bat:
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
