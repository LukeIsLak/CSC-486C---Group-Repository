using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public struct WeightedAttack
{
    public BatAttacks attack;
    public float weight;
    public WeightedAttack(BatAttacks attack, float weight)
    {
        this.attack = attack;
        this.weight = weight;
    }
}

public enum BatAttacks {
    PeckAttack,
    SwoopAttack
}

public enum BatStates {
    Spawn,
    Perching,
    Perched,
    Flutter,
    PeckAttacking,
    SwoopAttacking,
    PeckCompleteRebound,
    PeckIncompleteRebound,
    Hit,
    Wander
}

public class BatStateManager : StateMachine<Bat, BatStates>
{
    // public List<Bat> bats;
    // //XXX maybe move this to a scriptable object??
    // //XXX make an any aspect, so any state can transition into it
    // //XXX I would eventually like to move out of this, since this is not memory efficient, but for now
    // //XXX find a way to speed things up, N calculation for each bat in N*M lookup, more efficient way?
    // public Dictionary<BatStates, List<(BatStates toState, Func<Bat, bool> condition)>> conditionLookup = new Dictionary<BatStates, List<(BatStates, Func<Bat, bool>)>>();
    // public Dictionary<BatStates, Action<Bat>> enterStates = new Dictionary<BatStates, Action<Bat>>();
    // public Dictionary<BatStates, Action<Bat>> whileStates = new Dictionary<BatStates, Action<Bat>>();
    // public Dictionary<BatStates, Action<Bat>> exitStates = new Dictionary<BatStates, Action<Bat>>();

    public void Awake() {

        /*Spawn*/
        AddTransition(BatStates.Spawn, BatStates.Perching, SpawnToPerching);

        /*Perching*/
        AddTransition(BatStates.Perching, BatStates.Perched, PerchingToPerched);

        AddEnterState(BatStates.Perching, EnterPerchingState);
        AddWhileState(BatStates.Perching, WhilePerchingState);
        // AddExitState(BatStates.Perching, ExitPerchinState);

        /*Perched*/
        AddTransition(BatStates.Perched, BatStates.Flutter, PerchedToFlutterCondition);

        AddEnterState(BatStates.Perched, EnterPerchedState);
        AddExitState(BatStates.Perched, ExitPerchedState);
    
        /*Flutter*/

        //XXX maybe make a function were if the player goes out of range to wander away and then

        AddTransition(BatStates.Flutter, BatStates.PeckAttacking, FlutterToPeckCondition);
        AddTransition(BatStates.Flutter, BatStates.SwoopAttacking, FlutterToSwoopCondition);
        AddTransition(BatStates.Flutter, BatStates.Perching, FlutterToPerching);

        AddEnterState(BatStates.Flutter, EnterFlutterState);
        AddWhileState(BatStates.Flutter, WhileFlutterState);
        AddExitState(BatStates.Flutter, ExitFlutterState);

        /*Peck*/
        AddTransition(BatStates.PeckAttacking, BatStates.PeckCompleteRebound, PeckToPeckRebound);

        AddEnterState(BatStates.PeckAttacking, EnterPeckAttackState);
        AddWhileState(BatStates.PeckAttacking, WhilePeckAttackState);

        /*Peck Rebound*/
        AddTransition(BatStates.PeckCompleteRebound, BatStates.Flutter, PeckReboundToFlutter);

        AddEnterState(BatStates.PeckCompleteRebound, EnterPeckCompleteReboundState);
        AddWhileState(BatStates.PeckCompleteRebound, WhilePeckCompleteReboundState);
        AddExitState(BatStates.PeckCompleteRebound, ExitPeckReboundState);

        /*Swoop*/
        AddTransition(BatStates.SwoopAttacking, BatStates.Flutter, SwoopToFlutter);

        AddEnterState(BatStates.SwoopAttacking, EnterSwoopAttackState);
        AddWhileState(BatStates.SwoopAttacking, WhileSwoopAttackState);
        AddExitState(BatStates.SwoopAttacking, ExitSwoopAttackState);
    }

    public override void CheckTransition(Bat b) {
        BatStates currentState = b.currentState;
        if (!conditionLookup.TryGetValue(currentState, out var transitions)) {
            Debug.Log("No transitions exist from the current state!");
            return;
        }

        foreach(var (toState, condition) in transitions) {
            if (condition(b)) {
                if (exitStates.TryGetValue(currentState, out var exitFunc)) exitFunc(b);
                EnterUniversal(b);
                if (enterStates.TryGetValue(toState, out var enterFunc)) enterFunc(b);
                
                b.currentState = toState;
                break;
            }
        }
    }

    public override void CheckUpdate(Bat b) {
        var currentState = b.currentState;
        if (whileStates.TryGetValue(currentState, out var whileFunc)) {
            whileFunc(b);
        }
    }

    /******************************/
    /*   Transitions Conditions   */ 
    /******************************/

    /*From Spawn Transitions*/
    public bool SpawnToPerching(Bat b) {
        return true; // XXX For now, bats should just spawn and perch, this is not the behaviour we want in the long run
    }

    /*From Perching Transition*/
    public bool PerchingToPerched(Bat b) {
        return b.isPerched;
    }

    /* From Perched Transtions */
    public bool PerchedToFlutterCondition(Bat b) {
        return b.canLeavePerch && Vector3.Distance(b.gameObject.transform.position, b.playerTransform.position) <= b.bd.playerSearchDistance;
    }

    /* From Flutter Transtions */
    public bool FlutterToPeckCondition(Bat b) {
        return b.canAttack && !b.shouldPerch && b.nextAttack != null && b.nextAttack.Value == BatAttacks.PeckAttack;
    }

    public bool FlutterToSwoopCondition(Bat b) {
        return b.canAttack && !b.shouldPerch && b.nextAttack != null && b.nextAttack.Value == BatAttacks.SwoopAttack;
    }

    public bool FlutterToPerching(Bat b) {
        return b.shouldPerch;
    }

    /* From Peck Transtions */
    public bool PeckToPeckRebound(Bat b) {
        return b.peckComplete;
    }

    /* From PeckRebound Transitions */
    public bool PeckReboundToFlutter(Bat b) {
        return b.peckComplete == false;
    }

    /* From Swoop Transitions */
    public bool SwoopToFlutter(Bat b) {
        return b.swoopComplete;
    }

    /******************************/
    /*   Enter State Functions    */
    /******************************/

    public override void EnterUniversal(Bat b) {
        b.ChangeCurrentSpeed();
    }

    public void EnterPerchingState(Bat b) {
        b.findPerchSpot();
    }

    public void EnterPerchedState(Bat b) {
        b.isPerched = true;
        b.isMoving = false;
        b.isFluttering = false;
        b.isPecking = false;
        b.isPeckRebounding = false;

        b.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        b.anim.SetBool("IsPerched", true);
        
        b.StartPerchDuration(b.bd.perchDuration);
    }

    public void EnterFlutterState(Bat b) {
        b.StartAttackCooldown(b.bd.attackCooldown);
        b.StartFlutter(b.playerTransform);
    }

    public void EnterPeckAttackState(Bat b) {
        b.isAttacking = true;
        b.PeckTarget();
    }

    public void EnterSwoopAttackState(Bat b) {
        b.isAttacking = true;
        b.SwoopAttack();
    }

    public void EnterPeckCompleteReboundState(Bat b) {
        b.PeckRebound();
    }

    public void EnterPeckIncompleteReboundState(Bat b) {
        b.PeckRebound();
    }

    public void EnterHitState(Bat b) {
        // Implement hit logic if needed
    }

    public void EnterIdleState(Bat b) {
        b.isMoving = false;
        b.isFluttering = false;
        b.isPecking = false;
        b.isPeckRebounding = false;
    }

    /******************************/
    /*   While State Functions    */
    /******************************/

    public void WhilePerchingState(Bat b) {
        b.UpdatePerching();
    }

    public void WhileFlutterState(Bat b) {
        b.UpdateFlutter();
    }

    public void WhilePeckAttackState(Bat b) {
        b.UpdatePeck();
    }

    public void WhilePeckCompleteReboundState(Bat b) {
        b.UpdatePeckRebound();
    }

    public void WhileSwoopAttackState(Bat b) {
        b.UpdateSwoop();
    }


    /******************************/
    /*    Exit State Functions    */
    /******************************/

    public void ExitPerchedState(Bat b) {
        b.attackAmount = UnityEngine.Random.Range(b.bd.minAttackAmount, b.bd.maxAttackAmount);
        b.isAttacking = false;
        b.isPerched = false;

        b.rb.constraints &= ~RigidbodyConstraints.FreezeRotationX | ~RigidbodyConstraints.FreezeRotationZ;
        
        b.anim.SetBool("IsPerched", false);
        b.canLeavePerch = false;
    }

    public void ExitFlutterState(Bat b) {
        b.shouldPerch = false;
    }

    public void ExitPeckReboundState(Bat b) {
        b.peckComplete = false;
    }

    public void ExitSwoopAttackState(Bat b) {
        b.isAttacking = false;
        b.swoopComplete = false;
    }
}
