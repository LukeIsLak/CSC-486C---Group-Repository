using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float attackDelay = 1.0f;
    [SerializeField] private float attackDamage = 1.0f;
    [SerializeField] private int attackCount = 3;
    [SerializeField] private float attackRadius = 0.4f;
    [SerializeField] private float attackHitStopDuration = 0.03f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private SwordHitBox hitBoxA;
    [SerializeField] private SwordHitBox hitBoxB;
    [SerializeField] private SwordHitBox hitBoxC;
    // for increaseing player damage
    public float attackMultiplier = 1.0f;
    public int hasteCount = 0;
    private Vector3 lastPosition;
    public float movementSpeed;

    [Header("Dash")]
    [SerializeField] private float dashTime = 1.0f;
    [SerializeField] private float dashSpeed = 1.0f;
    [SerializeField] private float dashCD = 2.0f;
    public bool runawayBullActive = false;

    [Header("Animation State")]
    [SerializeField] private const string ATTACK1 = "SwingA";
    [SerializeField] private const string ATTACK2 = "SwingB";
    [SerializeField] private const string ATTACK3 = "SwingC";
    [SerializeField] private const string ATTACKIDLE = "Idle";

    [Header("References")]
    [SerializeField] private CharacterData playerData;
    [SerializeField] private GameEvent PlayerInteractEvent;
    [SerializeField] private GameEvent PlayerCloseInteractableMenu;

    private Animator swordAnimator;
    private Camera cam;
    private Coroutine hitStopCoroutine;
    private Coroutine increaseAttackSpeedRoutine;
    private PlayerController playerController;
    private Chest currentChest;
    private string currentAnimationState;
    private bool isAttacking;
    private float lastAttackTime;
    private int comboIndex = 0;

    //For dashing
    private bool isDashing;
    private float nextDashTime;
    public event System.Action<bool> OnDashNotify;
    private bool lastDashState = true;

    private float baseAttackSpeed = 1f;
    private float attackSpeedMultiplier = 1f;
    public float nextDashRemaining
    {
        get
        {
            if (Time.time >= nextDashTime) return 1f;
            return 1 - (nextDashTime - Time.time)/dashCD;
        }
    }
    private Coroutine dashCoroutine;
    // For checking when to queue and queue next attack
    private bool canQueue;
    private bool queuedNextAttack;
    private void Awake()
    {
        //swordAnimator = GetComponentInChildren<Animator>();
        cam = GetComponentInChildren<Camera>();
        playerController = GetComponent<PlayerController>();    
    }

    private void Update()
    {
        CheckChestInteractable();
        CheckDashStateForUI();
        if (hasteCount > 0) UpdateMovementSpeed();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isAttacking && canQueue)
        {
            queuedNextAttack = true;
            return;
        }
        Attack();
    }

    private void ChangeAnimationState(string newState)
    {
        if(currentAnimationState == newState) return;

        currentAnimationState = newState;
        Debug.Log(currentAnimationState);
        swordAnimator.CrossFadeInFixedTime(currentAnimationState, 0.2f);

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
    public void AttackRaycast()
    {
        if(Physics.SphereCast(cam.transform.position, attackRadius,cam.transform.forward,out RaycastHit hit, attackRange, enemyLayer))
        {
            //Debug.Log($"Hit: {hit.collider.name} ");

            var enemyComponent = hit.collider.GetComponentInParent<EnemyInterface>();
            if (enemyComponent != null) enemyComponent.GetComponent<FMODUnity.StudioEventEmitter>().Play(); enemyComponent.Hit(attackMultiplier * attackDamage);
            TriggerHitStop(attackHitStopDuration);
            
        }
    }

    private void TriggerHitStop(float duration)
    {
        if(hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(HitStopCoroutine(duration));
    }
    
    private IEnumerator HitStopCoroutine(float duration)
    {
        float originalSpeed = swordAnimator.speed;
        swordAnimator.speed = 0f;
        yield return new WaitForSecondsRealtime(duration);
        swordAnimator.speed = originalSpeed;
        hitStopCoroutine = null;
    }
    // call this in animation event
    public void EndAttack()
    {
        isAttacking = false;
        canQueue = false;
        if (queuedNextAttack)
        {
            queuedNextAttack = false;
            Attack();
            return;
        }
        ChangeAnimationState(ATTACKIDLE);
    }

    public void OpenComboWindow()
    {
        canQueue = true;
    }

    public void CloseComboWindow()
    {
        canQueue = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isDashing) return;
        if (Time.time < nextDashTime) return;    
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);

        dashCoroutine = StartCoroutine(Dash());

    }

    private IEnumerator Dash()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCD;
        float startTime = Time.time;
        while(Time.time < startTime + dashTime)
        {
            playerController.controller.Move(playerController.speed * GetDashDirection() * dashSpeed * Time.deltaTime);
            yield return null;
        }
        if(runawayBullActive){
            Collider[] hitColliders = Physics.OverlapSphere(cam.transform.position, 3f);
            //Debug.Log($"Hit: {hit.collider.name} ");

            foreach (var hit in hitColliders) {
                var enemyComponent = hit.GetComponent<Collider>().GetComponentInParent<EnemyInterface>();
                if (enemyComponent != null) enemyComponent.Hit((playerController.speed / 5) * 30, StatusEffectType.Knockback); // need some numbers decided
            }
        }
        isDashing = false;
        dashCoroutine = null;
    }
    private void CheckDashStateForUI() 
    {
        bool isReady = nextDashRemaining >= 1f;

        if (isReady != lastDashState)
        {
            lastDashState = isReady;
            OnDashNotify?.Invoke(isReady);
        }
    }


    private Vector3 GetDashDirection()
    {
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        right.y = 0;

        Vector3 direction = forward*playerController.moveDirection.y + right*playerController.moveDirection.x;

        if(direction.sqrMagnitude > 0.01f)
        {
            return direction.normalized;
        }

        return forward;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        currentChest?.Open();
        PlayerInteractEvent.Raise();
       
    }

    private void CheckChestInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 2.0f))
        {
            Chest chest = hit.collider.GetComponent<Chest>();
            if (chest != null && !chest.isOpened)
            {
                if(currentChest != chest)
                {
                    currentChest = chest;
                    UIManager.instance.ShowInteract();
                }
                return;
            }
        }
        currentChest = null;
        UIManager.instance?.HideInteract();
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        PlayerCloseInteractableMenu.Raise();
    }

    public void SetSwordAnimator(Animator swordAnimator)
    {
        this.swordAnimator = swordAnimator;
    }

    public void EnableSwordColiderA()
    {
        hitBoxA.EnableCollider();
    }

    public void DisableSwordColiderA()
    {
        hitBoxA.DisableCollider();
        hitBoxA.ResetHit();
    }
    public void EnableSwordColiderB()
    {
        hitBoxB.EnableCollider();
    }

    public void DisableSwordColiderB()
    {
        hitBoxB.DisableCollider();
        hitBoxB.ResetHit();
    }

    public void EnableSwordColiderC()
    {
        hitBoxC.EnableCollider();
    }

    public void DisableSwordColiderC()
    {
        hitBoxC.DisableCollider();
        hitBoxC.ResetHit();
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void IncreaseAttackSpeed(float increaseSpeedMultiplier)
    {
        attackSpeedMultiplier *= increaseSpeedMultiplier;
        swordAnimator.speed = baseAttackSpeed * attackSpeedMultiplier;

    }

    public void DecreaseAttackSpeed(float increaseSpeedMultiplier)
    {
        attackSpeedMultiplier /= increaseSpeedMultiplier;
        swordAnimator.speed = baseAttackSpeed * attackSpeedMultiplier;
    }

    private void UpdateMovementSpeed() 
    {
        float rawSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        float ease = 0.2f;
        movementSpeed = Mathf.Lerp(movementSpeed, rawSpeed, ease);
        lastPosition = transform.position;
    }
}
