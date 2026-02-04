using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 1.0f;
}
