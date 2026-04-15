using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkeletonRangedWeightedAttacks {
    public SkeletonRangedAttacks attack;
    public float weight;
    public Func<bool> condition;
    public SkeletonRangedWeightedAttacks(SkeletonRangedAttacks attack, float weight, Func<bool> condition) {
        this.attack = attack;
        this.weight = weight;
        this.condition = condition;
    }
}

public enum SkeletonRangedAttacks {
    AttackThrow,
    AttackSwing
}

public enum SkeletonRangedStates {
    Init,
    Spawn,
    SpawnWall,
    Idle,
    Wander,
    AgroApproach,
    ThrowAttack,
    SwingAttack,

    ActivateFiller,       // FILLER STATE
    DeactivateFiller,      // FILLER STATE
    Deactive,

}

public class SkeletonRangedStateMachine : StateMachine<SkeletonRanged, SkeletonRangedStates>
{
    public void Awake() {
        /*Init*/
        AddTransition(SkeletonRangedStates.Init, SkeletonRangedStates.Spawn, InitToSpawn);

        /*Spawn*/
        AddTransition(SkeletonRangedStates.Spawn, SkeletonRangedStates.SpawnWall, SpawnToWall);
        AddTransition(SkeletonRangedStates.Spawn, SkeletonRangedStates.Idle, SpawnToIdle);

        AddEnterState(SkeletonRangedStates.Spawn, EnterSpawnState);

        /*SpawnWall*/
        AddTransitionWithFiller(
            SkeletonRangedStates.SpawnWall, 
            SkeletonRangedStates.AgroApproach, 
            SkeletonRangedStates.ActivateFiller,
            WallToAgroApproach,
            ActivateFillerCond
        );
        // AddTransition(SkeletonRangedStates.SpawnWall, SkeletonRangedStates.AgroApproach, WallToAgroApproach);

        AddEnterState(SkeletonRangedStates.SpawnWall, EnterSpawnWallState);
        AddExitState(SkeletonRangedStates.SpawnWall, ExitSpawnWallState);

        /*Idle*/
        AddTransition(SkeletonRangedStates.Idle, SkeletonRangedStates.Wander, IdleToWander);
        AddTransition(SkeletonRangedStates.Idle, SkeletonRangedStates.AgroApproach, IdleToAgroApproach);

        AddEnterState(SkeletonRangedStates.Idle, EnterIdleState);

        /*Wander*/
        AddTransition(SkeletonRangedStates.Wander, SkeletonRangedStates.Idle, WanderToIdle);
        AddTransition(SkeletonRangedStates.Wander, SkeletonRangedStates.AgroApproach, WanderToAgroApproach);

        AddEnterState(SkeletonRangedStates.Wander, EnterWanderState);
        AddWhileState(SkeletonRangedStates.Wander, WhileWanderState);
        AddExitState(SkeletonRangedStates.Wander, ExitWanderState);

        /*AgroApproach*/
        AddTransition(SkeletonRangedStates.AgroApproach, SkeletonRangedStates.ThrowAttack, AgroApproachToThrow);
        AddTransition(SkeletonRangedStates.AgroApproach, SkeletonRangedStates.SwingAttack, AgroApproachToSwing);
        AddTransition(SkeletonRangedStates.AgroApproach, SkeletonRangedStates.Idle, AgroApproachToIdle);

        AddEnterState(SkeletonRangedStates.AgroApproach, EnterAgroApproachState);
        AddWhileState(SkeletonRangedStates.AgroApproach, WhileAgroApproachState);
        AddExitState(SkeletonRangedStates.AgroApproach, ExitAgroApproachState);

        /*ThrowAttack*/
        AddTransition(SkeletonRangedStates.ThrowAttack, SkeletonRangedStates.AgroApproach, ThrowToAgroApproach);
        AddTransition(SkeletonRangedStates.ThrowAttack, SkeletonRangedStates.Idle, ThrowToIdle);

        AddEnterState(SkeletonRangedStates.ThrowAttack, EnterThrowAttackState);
        // AddWhileState(SkeletonRangedStates.ThrowAttack, WhileThrowAttackState);
        AddExitState(SkeletonRangedStates.ThrowAttack, ExitThrowAttackState);

        /*SwingAttack*/    
        AddTransition(SkeletonRangedStates.SwingAttack, SkeletonRangedStates.AgroApproach, SwingToAgroApproach);
        AddTransition(SkeletonRangedStates.SwingAttack, SkeletonRangedStates.Idle, SwingToIdle);

        AddEnterState(SkeletonRangedStates.SwingAttack, EnterSwingAttackState);
        AddExitState(SkeletonRangedStates.SwingAttack, ExitSwingAttack);
    
        /*ActivateFiller*/
        AddEnterState(SkeletonRangedStates.ActivateFiller, EnterActivateFiller);
        AddExitState(SkeletonRangedStates.ActivateFiller, ExitActivateFiller);
    
        /*DeactivateFiller*/
        AddEnterState(SkeletonRangedStates.DeactivateFiller, EnterDeactivateFiller);
        AddExitState(SkeletonRangedStates.DeactivateFiller, ExitDeactivateFiller);
    }

    public override void CheckTransition(SkeletonRanged sr) {
        SkeletonRangedStates currentState = sr.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(sr)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(sr);
                EnterUniversal(sr);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(sr);
                
                sr.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(SkeletonRanged sr) {
        var currentState = sr.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(sr);
        }
    }

    public override void EnterUniversal(SkeletonRanged sr) {

    }

    /******************************/
    /*   Transitions Conditions   */ 
    /******************************/
    
    public bool InitToSpawn(SkeletonRanged sr) {
        return sr.isInitialized;
    }

    public bool SpawnToWall(SkeletonRanged sr) {
        return sr.inWallSpawn;
    }

    public bool SpawnToIdle(SkeletonRanged sr) {
        return sr.noWallSpot || !sr.inWallSpawn;
    }

    public bool WallToAgroApproach(SkeletonRanged sr) {
        return Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) <= sr.srd.detectionRadius;
    }

    public bool IdleToWander(SkeletonRanged sr) {
        return sr.canWander;
    }

    public bool IdleToAgroApproach(SkeletonRanged sr) {
        return Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) <= sr.srd.detectionRadius;
    }

    public bool WanderToIdle(SkeletonRanged sr) {
        return sr.doneWandering;
    }

    public bool WanderToAgroApproach(SkeletonRanged sr) {
        return Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) <= sr.srd.detectionRadius;
    }

    public bool AgroApproachToThrow(SkeletonRanged sr) {
        return sr.canAttack && sr.nextAttack != null && sr.nextAttack.Value == SkeletonRangedAttacks.AttackThrow;
    }

    public bool AgroApproachToSwing(SkeletonRanged sr) {
        return sr.canAttack && sr.nextAttack != null && sr.nextAttack.Value == SkeletonRangedAttacks.AttackSwing;
    }

    public bool AgroApproachToIdle(SkeletonRanged sr) {
        return Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) > sr.srd.detectionRadius;
    }

    public bool ThrowToAgroApproach(SkeletonRanged sr) {
        return !sr.isThrowing && Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) <= sr.srd.detectionRadius;
    }

    public bool ThrowToIdle(SkeletonRanged sr) {
        return !sr.isThrowing && Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) > sr.srd.detectionRadius;
    }

    public bool SwingToAgroApproach(SkeletonRanged sr) {
        return !sr.isSwinging && Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) <= sr.srd.detectionRadius;
    }

    public bool SwingToIdle(SkeletonRanged sr) {
        return !sr.isSwinging && Vector3.Distance(sr.gameObject.transform.position, sr.playerTransform.position) > sr.srd.detectionRadius;
    }

    public bool ActivateFillerCond(SkeletonRanged sr) {
        return sr.doneActivate;
    }

    public bool DeactivateFillerCond(SkeletonRanged sr) {
        return sr.doneDeactivate;
    }

    public bool ThrowCancel(SkeletonRanged sr) {
        return sr.canDeactivate;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public void EnterSpawnState(SkeletonRanged sr) {
        sr.InitializeSpawn();
    }

    public void EnterSpawnWallState(SkeletonRanged sr) {
        sr.anim.SetBool("isActive", false);
        sr.anim.SetTrigger("deactivate");
    }

    public void EnterIdleState(SkeletonRanged sr) {
        sr.isMoving = false;
        sr.anim.SetBool("isMoving", sr.isMoving);
        sr.StartIdleDuration();
    }

    public void EnterWanderState(SkeletonRanged sr) {
        sr.canWander = false;
        sr.doneWandering = false;
        sr.isMoving = true;
        sr.anim.SetBool("isMoving", sr.isMoving);
        Vector3 wanderTarget = sr.PickWanderSpotOnNavMesh(transform.position, sr.srd.wanderRadius); // Adjust radius as needed
        sr.nma.SetDestination(wanderTarget);
    }

    public void EnterAgroApproachState(SkeletonRanged sr) {
        sr.isAgro = true;
        sr.isMoving = true;
        sr.checkPlayerPath = true;
        sr.nextAttack = null;
        sr.anim.SetBool("isMoving", sr.isMoving);
        sr.WaitToAttack();
        sr.nma.SetDestination(sr.playerTransform.position);
    }

    public void EnterThrowAttackState(SkeletonRanged sr) {
        sr.isAttacking = true;
        sr.anim.SetBool("throwAttack", true);
        sr.StartThrowThrough(sr.playerTransform.position);
    }

    public void EnterSwingAttackState(SkeletonRanged sr) {
        sr.isAttacking = true;
        sr.anim.SetBool("normalAttack", true);
        sr.StartSwingTimer();
    }

    public void EnterActivateFiller(SkeletonRanged sr) {
        sr.anim.SetBool("isActive", true);
        sr.anim.SetTrigger("activate");
        sr.StartActivate();
    }

    public void EnterDeactivateFiller(SkeletonRanged sr) {
        sr.anim.SetBool("isActive", false);
        sr.anim.SetTrigger("deactivate");
        sr.StartDeactivate();
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileWanderState(SkeletonRanged sr) {
        sr.UpdateWander();
        sr.CheckWanderDone();
    }

    public void WhileAgroApproachState(SkeletonRanged sr) {
        sr.UpdateAgroApproach();
        sr.nma.nextPosition = sr.gameObject.transform.position;
    }

    /******************************/
    /*   Leave State Functions    */
    /******************************/

    public void ExitSpawnWallState(SkeletonRanged sr) {
        sr.canDeactivate = false;
        sr.GetOffWall();
    }

    public void ExitWanderState(SkeletonRanged sr) {
        sr.doneWandering = false;
        sr.canWander = false;
    }

    public void ExitAgroApproachState(SkeletonRanged sr) {
        sr.isAgro = false;
    }

    public void ExitThrowAttackState(SkeletonRanged sr) {
        sr.isAttacking = false;
        sr.isThrowing = false;
        sr.anim.SetBool("throwAttack", false);
        sr.anim.SetTrigger("doneAttack");
    }

    public void ExitSwingAttack(SkeletonRanged sr) {
        sr.isAttacking = false;
        sr.anim.SetBool("normalAttack", false);
        sr.anim.SetTrigger("doneAttack");
    }

    public void ExitActivateFiller(SkeletonRanged sr) {
        sr.doneActivate = false;
        sr.anim.SetBool("isActive", true);
    }

    public void ExitDeactivateFiller(SkeletonRanged sr) {
        sr.canDeactivate = false;
        sr.doneDeactivate = false;
    }
}
