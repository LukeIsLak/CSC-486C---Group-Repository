using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/RatData")]
public class RatBaseData : BaseEnemyData
{

    [Header("Speed Multipliers")]
    public float moveSpeed = 3f;
}
