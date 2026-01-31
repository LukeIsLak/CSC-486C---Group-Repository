using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.0f;
    [SerializeField] private float attackDelay = 1.0f;
    [SerializeField] private float attackSpeed = 1.0f;
    [SerializeField] private float attackDamage = 1.0f;
    [SerializeField] private int attackCount = 3;

    [Header("Animation State")]
    [SerializeField] private const string ATTACK1 = "Attack 1";
    [SerializeField] private const string ATTACK2 = "Attack 2";
    [SerializeField] private const string ATTACK3 = "Attack 3";
    [SerializeField] private const string ATTACKIDLE = "Idle";

    private Animator animator;
    private Camera cam;
    private LayerMask enemyLayer;

    private string currentAnimationState;
    private bool isAttacking;
    private float lastAttackTime;
    private int comboIndex = 0;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        cam = GetComponentInChildren<Camera>();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) Attack();
    }

    private void ChangeAnimationState(string newState)
    {
        if(currentAnimationState == newState) return;

        currentAnimationState = newState;
        //Debug.Log(currentAnimationState);
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);

    }

    private void Attack()
    {
        if (isAttacking) return;

        if(Time.time - lastAttackTime > attackDelay)
        {
            comboIndex = 0;
        }

        comboIndex++;

        if(comboIndex > attackCount) comboIndex = 1;

        lastAttackTime = Time.time;
        isAttacking = true;
        switch (comboIndex)
        {
            case 1:
                ChangeAnimationState(ATTACK1);
                break;
            case 2:
                ChangeAnimationState(ATTACK2);
                break;
            case 3:
                ChangeAnimationState(ATTACK3);
                break;
        }
    }

    // call this in animation event
    public void Raycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, enemyLayer))
        {
            // Deal damage to hit
        }
    }
    
    // call this in animation event
    public void ResetAttack()
    {
        isAttacking = false;
        ChangeAnimationState(ATTACKIDLE);
    }
}
