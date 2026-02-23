using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldRat : EnemyInterface
{
    public float detectionRadius    = 5;
    public float leapRadius         = 2;
    public float leapSpeedRatio     = 4;
    public float leapDuration       = 0.5f;
    public float leapCooldown       = 3f;

    public bool isLeaping          = false;
    public bool canLeap            = true;
    public float leapForce         = 20;
    public float leapYIncrease     = 0.25f;
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
        if (posDiff.magnitude == 0) return;
        Vector3 direction   = posDiff / posDiff.magnitude;

        // Preserve y velocity
        float velocityY     = rb.velocity.y;

        // Set forward to always face player
        transform.forward   = direction;

        if (isLeaping) return;
        // Do nothing if out of range of the player
        if (posDiff.magnitude > detectionRadius) 
        {
            desiredVel = Vector3.zero;
            return;
        }

        // Get movement direction
        if (direction.magnitude == 0) return;
        direction = direction / direction.magnitude;

        // We are in leap range
        if (posDiff.magnitude < leapRadius && canLeap)
        {
            curLeapDir = direction;
            curLeapDir[1] += leapYIncrease;
            StartCoroutine(DoLeap());
            return;
        }

        // We are close but not in leap range, so home in
        direction[1] = 0f;
        desiredVel = direction * moveSpeed;
        desiredVel.y = velocityY;
        rb.MovePosition(transform.position + desiredVel * Time.fixedDeltaTime);
    }

    // Set and unset relevant flags after timings met
    private IEnumerator DoLeap()
    {
        isLeaping = true;
        canLeap = false;
        rb.AddForce(curLeapDir * leapForce);
        yield return new WaitForSeconds(leapDuration);
        isLeaping = false;
        yield return new WaitForSeconds(leapCooldown);
        canLeap = true;
    }

    public void UpdateSpeed()
    {
        moveSpeed = enemyData.baseMoveSpeed * playerData.moveSpeed * enemyEffects.getEnemySpeedModifier();
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
