using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/BatData")]
public class BatBaseData : BaseEnemyData
{

    [Header("Speed Multipliers")]
    public float moveSpeed = 3f;
    public float flutterMoveSpeed = 4f;
    public float peckSpeed = 5f;
    public float peckReboundSpeed = 10f;
    public float swoopSpeed = 5f;

    [Header("Bat Motion Parameters")]
    public int numPathPoint = 4;
    public float pointTolerance = 0.05f;

    [Header("Ceiling Location Possibilities Parameters")]
    public int radialSteps = 1;
    public int angularSteps = 10;
    public float coneAngle = 45f;
    public float maxDistance = 10f;
    public float maxColliderAngle = 45f;
    public LayerMask layerMask = ~0;
    public float pearchYOffset = -0.5f;

    /*For more complex meshes, determine perfered perchin spot*/
    [Header("Ceiling Position Weights")]
    public float normalWeight = 1f;
    public float heightWeight = 1f;
    public float upAngleWeight = 1f;

    [Header("Flutter Parameters")]   
    public float flutterRadius = 3f;                // average orbit radius
    public float flutterAngularSpeed = 90f;         // degrees per second
    public float flutterRadialJitter = 0.5f;        // random variation in radius
    public float verticalBobAmplitude = 0.4f;       // vertical bob amount
    public float verticalBobSpeed = 2f;             // vertical bob speed
    public float horizontalBobAmplitude = 0.4f;     // horizontal bob amount
    public float horizontalBobSpeed = 2f;           // horizontal bob speed
    public float flutterTurnSpeed = 5f;             // rotation smoothing
    public float flutterBaseHeight = 2f;            // base flutter height from (0, 0, 0)
    public float lateralSmoothing = 8f;             // Smoothing for horizontal squiggle to reduce spikiness (higher = smoother)

    [Header("Bat Peck Variables")]
    public float reboundHeight = 1f;                // rebound height when attack complete
    public float reboundIncompleteHeight = 1f;      // rebound height when attack incomplete
    public float minRebountHeight = 1f;             // at a minimum, bounce bat this high
    public float reboundDistance = 1f;              // distance to bounce back horizontally
    public float reboundIncompleteDistance = 1f;    // incomplete as in attack doesn't finish

    [Header("Bat Transition Variables")]
    public float playerSearchDistance = 5;
    public int minAttackAmount = 1;
    public int maxAttackAmount = 3;

    public float perchDuration = 2f;

    [Header("Steer Variables")]
    public int steerSteps = 1;
    public int steerAngularSteps = 10;
    public float steerViewAngle = 45f;
    public float maxSteerDistance = 10f;
    public float steerDistanceWeight = 2f;
    public float steerDirectionWeight = 1f;
    public LayerMask steerLayerMask = ~0;



    [Header("Attack Variables")]
    public float attackCooldown = 2f;

    public List<WeightedAttack> weightedAttacks = new List<WeightedAttack>
    {
        new WeightedAttack(BatAttacks.PeckAttack, 2f),
        new WeightedAttack(BatAttacks.SwoopAttack, 1f)
    };
}
