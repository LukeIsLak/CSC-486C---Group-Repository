using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/BaseEnemyData")]
public class BaseEnemyData : ScriptableObject
{
    // These are in Percents
    public int baseHealth;

    public float baseMoveSpeed;
}
