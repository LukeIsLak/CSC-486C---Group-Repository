using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/RatData")]
public class RatBaseData : BaseEnemyData
{

    [Header("Speed Multipliers")]
    public float moveSpeed = 3f;

    [Header("Navigation Weight Values")]
    public float navWeight          = 1.5f;
    public float separationWeight   = 2f;
    public float alignmentWeight    = 1f;
    public float cohesionWeight     = 1f;
    public float wanderWeight       = 0.3f;
    public float destStopDist       = 1f;

    [Header("Navigation Misc. Values")]
    public float minAddIdleWait = 0f;
    public float maxAddIdleWait = 2f;

    [Header("Leaping Values")]
    public float leapRadius     = 2f;
    public float leapSpeedRatio = 4f;
    public float leapForce      = 20f;
    public float leapYIncrease  = 0.25f;
    public float leapDuration   = 0.5f;
    public float leapCooldown   = 3f;

    [Header("Neighbour or Colony Values")]
    public LayerMask ratMask = ~0;    
    public float colonyDist = 5f;
    public float neighbourRadius = 5f;
    public float neighbourStopRadius = 2f;

    [Header("Agro Values")]
    public float checkPlayerUpdate = 0.1f;
    public float detectionRadius = 5f;
    public float wanderRadius = 4f;

    [Header("Misc. Rat Information")]
    public float idleDuration = 2f;
    public float minMoveWait = 0.25f;
    public float maxMoveWait = 0.75f;
}
