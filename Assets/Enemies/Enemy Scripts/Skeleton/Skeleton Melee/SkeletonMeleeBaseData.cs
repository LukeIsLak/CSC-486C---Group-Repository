using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Skeleton/Melee")]
public class SkeletonMeleeBaseData : BaseEnemyData
{
    [Header("Speed Multipliers")]
    public float moveSpeed = 3f;

    [Header("Navigation Weight Values")]
    public float navWeight          = 1.5f;
    public float separationWeight   = 2f;
    public float alignmentWeight    = 1f;
    public float cohesionWeight     = 1f;
    public float wanderWeight       = 0.3f;
    public float destStopDist       = 2f;

    [Header("Neighbour Values")]
    public LayerMask skeletonMask = ~0;
    public float neighbourRadius = 5f;
    public float neighbourStopRadius = 2f;

    [Header("Skeleton Spawn Values")]
    public float wallSpawnChance = 0.5f;

    [Header("Skeleton Agro Values")]
    public float detectionRadius = 10f;
}
