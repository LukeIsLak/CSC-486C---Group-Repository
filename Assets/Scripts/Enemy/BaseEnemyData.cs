using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyData : ScriptableObject
{
    [Header("Base Stats - In % Compared to Player")]
    public float baseHealth;
    public float baseMoveSpeed;
}
