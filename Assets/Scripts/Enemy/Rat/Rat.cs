using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyInterface
{
    [Header("Rat Base Fields")]
    public Animator anim;
    public Rigidbody rb;
    public RatBaseData rd;
    public Transform playerTransform;

    // [Header("State Checkers")]

    [Header("Misc. Variables")]
    public RatStates currentState;
}
