using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(UnityEngine.AI.NavMeshAgent))]
public class SkeletonMelee : EnemyInterface
{
    // [Header("Skeleton Meele Base Fields")]

    // [Header("State Checkers")]

    [Header("Misc. Variables")]
    public SkeletonMeleeStates currentState;
}
