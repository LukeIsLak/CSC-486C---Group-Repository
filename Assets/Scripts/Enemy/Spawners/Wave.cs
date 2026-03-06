using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/Wave")]
public class Wave : ScriptableObject
{
    public List<GameObject> enemyTypes;
    public List<int> enemyCounts;
    
    private void OnValidate() {
        if (enemyTypes != null && enemyCounts != null && enemyTypes.Count != enemyCounts.Count) {
            Debug.LogError($"Wave ScriptableObject: enemyTypes.Count ({enemyTypes.Count}) does not match enemySpawns.Count ({enemyCounts.Count})", this);
        }
    }
}
