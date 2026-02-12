using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : EnemyInterface
{
    public EnemyEffects enemyEffects;

    public float detectionRadius    = 5;
    public float leapRadius         = 2;
    public float leapSpeedRatio     = 4;
    public float leapDuration       = 0.5f;
    public float leapCooldown       = 3f;

    public bool isLeaping          = false;
    public bool canLeap            = true;
    
    public Vector3 curLeapDir;
    public Vector3 desiredVel;
    
    private float baseMoveSpeed;
    
    private GameObject playerGO;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
        playerGO = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        isLeaping = false;
        canLeap = true;
    }

    // Update is called once per frame
    void Update()
    {
        playerGO = GameObject.FindWithTag("Player");
        if (playerGO == null) return;

        Vector3 posDiff     = playerGO.transform.position - transform.position;
        Vector3 direction   = posDiff / posDiff.magnitude;

        // Preserve y velocity
        float velocityY     = rb.velocity.y;

        // Set forward to always face player
        transform.forward   = direction;

        // If we're leaping, we continue no matter what.
        if (isLeaping)
        {
            desiredVel = curLeapDir * moveSpeed * leapSpeedRatio;
            desiredVel.y = velocityY;
            rb.velocity = desiredVel;
            return;
        }

        // Do nothing if out of range of the player
        if (posDiff.magnitude > detectionRadius) 
        {
            desiredVel = Vector3.zero;
            return;
        }

        // Get movement direction
        direction[1] = 0f;
        direction = direction / direction.magnitude;

        // We are in leap range
        if (posDiff.magnitude < leapRadius && canLeap)
        {
            curLeapDir = direction;
            // Better approach: impulse of velocity up + towards player
            // Then don't let move unless on ground.
            // Set canLeap accordingly, but isLeaping is then not needed.
            StartCoroutine(DoLeap());
            return;
        }

        // We are close but not in leap range, so home in
        desiredVel = direction * moveSpeed;
        desiredVel.y = velocityY;
        rb.velocity = desiredVel;
    }

    // Set and unset relevant flags after timings met
    private IEnumerator DoLeap()
    {
        canLeap = false;
        isLeaping = true;
        yield return new WaitForSeconds(leapDuration);
        isLeaping = false;
        yield return new WaitForSeconds(leapCooldown);
        canLeap = true;
    }

    public void UpdateSpeed()
    {
        moveSpeed = enemyData.baseMoveSpeed * enemyEffects.getEnemySpeedModifier();
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject == playerGO)
        {
            Health hc = playerGO.GetComponent<Health>();
            hc.TakeDamage(playerData.maxHealth * 0.05f);
        }
    }
}
