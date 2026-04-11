using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonMeleeStates {
    Init,
    Spawn,
    SpawnWall,
    Idle,
    Wander,
    AgroApproach,
    DashAttack,
    SwingAttack
}

public class SkeletonMeleeStateMachine : StateMachine<SkeletonMelee, SkeletonMeleeStates>
{
    public void Awake() {
        /*Init*/
        AddTransition(SkeletonMeleeStates.Init, SkeletonMeleeStates.Spawn, InitToSpawn);

        /*Spawn*/
        AddTransition(SkeletonMeleeStates.Spawn, SkeletonMeleeStates.SpawnWall, SpawnToWall);
        AddTransition(SkeletonMeleeStates.Spawn, SkeletonMeleeStates.Idle, SpawnToIdle);

        AddEnterState(SkeletonMeleeStates.Spawn, EnterSpawnState);

        /*SpawnWall*/
        AddTransition(SkeletonMeleeStates.SpawnWall, SkeletonMeleeStates.AgroApproach, WallToAgroApproach);

        AddExitState(SkeletonMeleeStates.SpawnWall, ExitSpawnWallState);

        /*Idle*/
        AddTransition(SkeletonMeleeStates.Idle, SkeletonMeleeStates.Wander, IdleToWander);
        AddTransition(SkeletonMeleeStates.Idle, SkeletonMeleeStates.AgroApproach, IdleToAgroApproach);

        /*Wander*/
        AddTransition(SkeletonMeleeStates.Wander, SkeletonMeleeStates.Idle, WanderToIdle);
        AddTransition(SkeletonMeleeStates.Wander, SkeletonMeleeStates.AgroApproach, WanderToAgroApproach);

        /*AgroApproach*/
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.DashAttack, AgroApproachToDash);
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.SwingAttack, AgroApproachToSwing);
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.Idle, AgroApproachToIdle);

        /*DashAttack*/
        AddTransition(SkeletonMeleeStates.DashAttack, SkeletonMeleeStates.AgroApproach, DashToAgroApproach);
        AddTransition(SkeletonMeleeStates.DashAttack, SkeletonMeleeStates.Idle, DashToIdle);

        /*SwingAttack*/    
        AddTransition(SkeletonMeleeStates.SwingAttack, SkeletonMeleeStates.AgroApproach, SwingToAgroApproach);
        AddTransition(SkeletonMeleeStates.SwingAttack, SkeletonMeleeStates.Idle, SwingToIdle);
    }

    public override void CheckTransition(SkeletonMelee sm) {
        SkeletonMeleeStates currentState = sm.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(sm)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(sm);
                EnterUniversal(sm);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(sm);
                
                sm.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(SkeletonMelee sm) {
        var currentState = sm.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(sm);
        }
    }

    public override void EnterUniversal(SkeletonMelee sm) {

    }

    /******************************/
    /*   Transitions Conditions   */ 
    /******************************/
    
    public bool InitToSpawn(SkeletonMelee sm) {
        return sm.isInitialized;
    }

    public bool SpawnToWall(SkeletonMelee sm) {
        return sm.inWallSpawn;
    }

    public bool SpawnToIdle(SkeletonMelee sm) {
        return sm.noWallSpot || !sm.inWallSpawn;
    }

    public bool WallToAgroApproach(SkeletonMelee sm) {
        return Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) <= sm.smd.detectionRadius;
    }

    public bool IdleToWander(SkeletonMelee sm) {
        return false;
    }

    public bool IdleToAgroApproach(SkeletonMelee sm) {
        return false;
    }

    public bool WanderToIdle(SkeletonMelee sm) {
        return false;
    }

    public bool WanderToAgroApproach(SkeletonMelee sm) {
        return false;
    }

    public bool AgroApproachToDash(SkeletonMelee sm) {
        return false;
    }

    public bool AgroApproachToSwing(SkeletonMelee sm) {
        return false;
    }

    public bool AgroApproachToIdle(SkeletonMelee sm) {
        return false;
    }

    public bool DashToAgroApproach(SkeletonMelee sm) {
        return false;
    }

    public bool DashToIdle(SkeletonMelee sm) {
        return false;
    }

    public bool SwingToAgroApproach(SkeletonMelee sm) {
        return false;
    }

    public bool SwingToIdle(SkeletonMelee sm) {
        return false;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public void EnterSpawnState(SkeletonMelee sm) {
        sm.InitializeSpawn();
    }

    /******************************/
    /*   Leave State Functions    */
    /******************************/

    public void ExitSpawnWallState(SkeletonMelee sm) {
        sm.GetOffWallAtAngle();
    }
}
