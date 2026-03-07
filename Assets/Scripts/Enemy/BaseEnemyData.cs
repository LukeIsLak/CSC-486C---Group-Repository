using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/BaseEnemyData")]
public class BaseEnemyData : ScriptableObject
{
    [Header("Base Stats - In % Compared to Player")]
    public float baseHealth;
    public float baseMoveSpeed;

    [Header("Damage Stats - In #")]
    public float damage;
}
