using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkeletonMeleeWeightedAttacks {
    public SkeletonMeleeAttacks attack;
    public float weight;
    public Func<bool> condition;
    public SkeletonMeleeWeightedAttacks(SkeletonMeleeAttacks attack, float weight, Func<bool> condition) {
        this.attack = attack;
        this.weight = weight;
        this.condition = condition;
    }
}

public enum SkeletonMeleeAttacks {
    AttackDash,
    AttackSwing
}

public enum SkeletonMeleeStates {
    Init,
    Spawn,
    SpawnWall,
    Idle,
    Wander,
    AgroApproach,
    DashAttack,
    SwingAttack,

    ActivateFiller,       // FILLER STATE
    DeactivateFiller,      // FILLER STATE
    Deactive,

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
        AddTransitionWithFiller(
            SkeletonMeleeStates.SpawnWall, 
            SkeletonMeleeStates.AgroApproach, 
            SkeletonMeleeStates.ActivateFiller,
            WallToAgroApproach,
            ActivateFillerCond
        );
        // AddTransition(SkeletonMeleeStates.SpawnWall, SkeletonMeleeStates.AgroApproach, WallToAgroApproach);

        AddEnterState(SkeletonMeleeStates.SpawnWall, EnterSpawnWallState);
        AddExitState(SkeletonMeleeStates.SpawnWall, ExitSpawnWallState);

        /*Idle*/
        AddTransition(SkeletonMeleeStates.Idle, SkeletonMeleeStates.Wander, IdleToWander);
        AddTransition(SkeletonMeleeStates.Idle, SkeletonMeleeStates.AgroApproach, IdleToAgroApproach);

        AddEnterState(SkeletonMeleeStates.Idle, EnterIdleState);

        /*Wander*/
        AddTransition(SkeletonMeleeStates.Wander, SkeletonMeleeStates.Idle, WanderToIdle);
        AddTransition(SkeletonMeleeStates.Wander, SkeletonMeleeStates.AgroApproach, WanderToAgroApproach);

        AddEnterState(SkeletonMeleeStates.Wander, EnterWanderState);
        AddWhileState(SkeletonMeleeStates.Wander, WhileWanderState);
        AddExitState(SkeletonMeleeStates.Wander, ExitWanderState);

        /*AgroApproach*/
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.DashAttack, AgroApproachToDash);
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.SwingAttack, AgroApproachToSwing);
        AddTransition(SkeletonMeleeStates.AgroApproach, SkeletonMeleeStates.Idle, AgroApproachToIdle);

        AddEnterState(SkeletonMeleeStates.AgroApproach, EnterAgroApproachState);
        AddWhileState(SkeletonMeleeStates.AgroApproach, WhileAgroApproachState);
        AddExitState(SkeletonMeleeStates.AgroApproach, ExitAgroApproachState);

        /*DashAttack*/
        AddTransition(SkeletonMeleeStates.DashAttack, SkeletonMeleeStates.AgroApproach, DashToAgroApproach);
        AddTransition(SkeletonMeleeStates.DashAttack, SkeletonMeleeStates.Idle, DashToIdle);
        
        // AddMultTransitionWithFiller(
        //     SkeletonMeleeStates.DashAttack, 
        //     SkeletonMeleeStates.AgroApproach, 
        //     SkeletonMeleeStates.DeactivateFiller,
        //     StopDashAttack,
        //     ActivateFillerCond
        // );

        AddEnterState(SkeletonMeleeStates.DashAttack, EnterDashAttackState);
        // AddWhileState(SkeletonMeleeStates.DashAttack, WhileDashAttackState);
        AddExitState(SkeletonMeleeStates.DashAttack, ExitDashAttackState);

        /*SwingAttack*/    
        AddTransition(SkeletonMeleeStates.SwingAttack, SkeletonMeleeStates.AgroApproach, SwingToAgroApproach);
        AddTransition(SkeletonMeleeStates.SwingAttack, SkeletonMeleeStates.Idle, SwingToIdle);
    
        /*ActivateFiller*/
        AddEnterState(SkeletonMeleeStates.ActivateFiller, EnterActivateFiller);
        AddExitState(SkeletonMeleeStates.ActivateFiller, ExitActivateFiller);
    
        /*DeactivateFiller*/
        AddEnterState(SkeletonMeleeStates.DeactivateFiller, EnterDeactivateFiller);
        AddExitState(SkeletonMeleeStates.DeactivateFiller, ExitDeactivateFiller);
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
        return sm.canWander;
    }

    public bool IdleToAgroApproach(SkeletonMelee sm) {
        return Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) <= sm.smd.detectionRadius;
    }

    public bool WanderToIdle(SkeletonMelee sm) {
        return sm.doneWandering;
    }

    public bool WanderToAgroApproach(SkeletonMelee sm) {
        return Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) <= sm.smd.detectionRadius;
    }

    public bool AgroApproachToDash(SkeletonMelee sm) {
        return sm.canAttack == true && sm.nextAttack != null && sm.nextAttack.Value == SkeletonMeleeAttacks.AttackDash;
    }

    public bool AgroApproachToSwing(SkeletonMelee sm) {
        return false;
    }

    public bool AgroApproachToIdle(SkeletonMelee sm) {
        return Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) > sm.smd.detectionRadius;
    }

    public bool DashToAgroApproach(SkeletonMelee sm) {
        return sm.isDashing == false && Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) <= sm.smd.detectionRadius;
    }

    public bool DashToIdle(SkeletonMelee sm) {
        return sm.isDashing == false && Vector3.Distance(sm.gameObject.transform.position, sm.playerTransform.position) > sm.smd.detectionRadius;
    }

    public bool SwingToAgroApproach(SkeletonMelee sm) {
        return false;
    }

    public bool SwingToIdle(SkeletonMelee sm) {
        return false;
    }

    public bool ActivateFillerCond(SkeletonMelee sm) {
        return sm.doneActivate;
    }

    public bool DeactivateFillerCond(SkeletonMelee sm) {
        return sm.doneDeactivate;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public void EnterSpawnState(SkeletonMelee sm) {
        sm.InitializeSpawn();
    }

    public void EnterSpawnWallState(SkeletonMelee sm) {
        sm.anim.SetBool("isActive", false);
        sm.anim.SetTrigger("deactivate");
    }

    public void EnterIdleState(SkeletonMelee sm) {
        sm.isMoving = false;
        sm.anim.SetBool("isMoving", sm.isMoving);
        sm.StartIdleDuration();
    }

    public void EnterWanderState(SkeletonMelee sm) {
        sm.canWander = false;
        sm.doneWandering = false;
        sm.isMoving = true;
        sm.anim.SetBool("isMoving", sm.isMoving);
        Vector3 wanderTarget = sm.PickWanderSpotOnNavMesh(transform.position, sm.smd.wanderRadius); // Adjust radius as needed
        sm.nma.SetDestination(wanderTarget);
    }

    public void EnterAgroApproachState(SkeletonMelee sm) {
        sm.isAgro = true;
        sm.isMoving = true;
        sm.anim.SetBool("isMoving", sm.isMoving);
        sm.checkPlayerPath = true;
        sm.nextAttack = null;
        sm.WaitToAttack();
        sm.nma.SetDestination(sm.playerTransform.position);
    }

    public void EnterDashAttackState(SkeletonMelee sm) {
        sm.anim.SetBool("dashAttack", true);
        sm.StartDashThrough(sm.playerTransform.position);
    }

    public void EnterActivateFiller(SkeletonMelee sm) {
        sm.anim.SetTrigger("activate");
        sm.StartActivate();
    }

    public void EnterDeactivateFiller(SkeletonMelee sm) {
        sm.StartDeactivate();
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhileWanderState(SkeletonMelee sm) {
        sm.UpdateWander();
        sm.CheckWanderDone();
    }

    public void WhileAgroApproachState(SkeletonMelee sm) {
        sm.UpdateAgroApproach();
        sm.nma.nextPosition = sm.gameObject.transform.position;
    }

    /******************************/
    /*   Leave State Functions    */
    /******************************/

    public void ExitSpawnWallState(SkeletonMelee sm) {
        sm.GetOffWall();
    }

    public void ExitWanderState(SkeletonMelee sm) {
        sm.doneWandering = false;
        sm.canWander = false;
    }

    public void ExitAgroApproachState(SkeletonMelee sm) {
        sm.isAgro = false;
    }

    public void ExitActivateFiller(SkeletonMelee sm) {
        sm.anim.SetBool("isActive", true);
        sm.doneActivate = false;
    }

    public void ExitDashAttackState(SkeletonMelee sm) {
        sm.anim.SetBool("dashAttack", false);
        sm.anim.SetTrigger("dashAttack");
        sm.rb.velocity = Vector3.zero;
        sm.isDashing = false;
    }

    public void ExitDeactivateFiller(SkeletonMelee sm) {
        sm.doneDeactivate = false;
    }
}
