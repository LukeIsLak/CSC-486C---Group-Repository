using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/VoidKnightData")]
public class VoidKnightBaseData : BaseEnemyData
{
    [Header("Player Visibility")]
    public float sightDistance  = 20f;
    public float sightAngle     = 60f;
    public int viewMask;


    [Header("Navigation Values")]
    public float destStopDist       = 1f;

    [Header("Agro Values")]
    public float checkPlayerUpdate = 0.1f;
    public float detectionRadius = 5f;
    public float wanderRadius = 4f;

    public float dissolveTime = 1.0f;
}
