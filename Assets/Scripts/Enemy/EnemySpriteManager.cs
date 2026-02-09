using UnityEngine;

public class EnemySpriteManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite sideSprite;
    public Sprite backSprite;

    [Header("Sprite Settings")]
    [Tooltip("Dot threshold above which the player is considered facing the enemy (front)")]
    public float frontDotThreshold = 0.5f;
    [Tooltip("Dot threshold below which the player is considered facing away from the enemy (back)")]
    public float backDotThreshold = -0.5f;
    [Tooltip("Seconds between automatic sprite updates. Increase to reduce cost.")]
    public float spriteUpdateInterval = 0.1f;

    [Header("Optional Rotator")]
    [Tooltip("An optional child object that will rotate to face the player regardless of the main object's facing.")]
    public Transform rotator;
    [Tooltip("If true, only rotate around Y axis (useful for top-down) ")]
    public bool rotatorOnlyYaw = true;
    [Tooltip("How quickly the rotator snaps to face the player (higher = faster)")]
    public float rotatorLerpSpeed = 10f;
    [Tooltip("Yaw offset in degrees applied after facing the target (e.g. +90)")]
    public float rotatorXawOffsetDeg = -90f;

    private float spriteTimer = 0f;

    public Vector3 offset;

    void Reset()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) Debug.LogError($"EnemySpriteManager on '{name}' has no SpriteRenderer assigned or found in children.");

        if (playerTransform == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        UpdateSpriteImmediate();
    }

    void Update()
    {
        if (rotator != null) RotateRotatorToFacePlayer();

        if (playerTransform == null || spriteRenderer == null) return;

        spriteTimer += Time.deltaTime;
        if (spriteTimer < spriteUpdateInterval) return;
        spriteTimer = 0f;
        UpdateSpriteImmediate();
    }

    private void UpdateSpriteImmediate()
    {
        if (playerTransform == null || spriteRenderer == null) return;

        // Evaluate target (player + offset)
        Vector3 target = playerTransform.position + offset;

        // Vector from this object to the player
        Vector3 toPlayer = target - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 1e-6f) return;
        toPlayer.Normalize();

        // Use this GameObject's rotation as the facing reference (ignore rotator for sprite selection)
        Transform facing = transform;
        Vector3 facingForward = transform.forward;
        facingForward.y = 0f;
        if (facingForward.sqrMagnitude < 1e-6f) facingForward = facing.right;
        facingForward.Normalize();

        // Angle: 0 means player is directly in front of the facing transform
        float angle = Vector3.SignedAngle(facingForward, toPlayer, Vector3.up); // -180..180

        Sprite chosen = null;
        // front: [-45, 45]
        if (angle >= -45f && angle <= 45f)
        {
            chosen = frontSprite;
        }
        // side: (45,135] or [-135,-45)
        else if ((angle > 45f && angle <= 135f) || (angle < -45f && angle >= -135f))
        {
            chosen = sideSprite;
        }
        // back: the remaining arc (>135 or <-135)
        else
        {
            chosen = backSprite;
        }

        if (chosen == null) return;
        if (spriteRenderer.sprite != chosen)
        {
            Debug.Log($"EnemySpriteManager: switching sprite on '{name}' to '{chosen.name}'");
            spriteRenderer.sprite = chosen;
        }

        // Flip sprite horizontally so it faces the correct left/right side relative to this transform
        bool playerIsRightOfFacing = Vector3.Dot(transform.right, toPlayer) > 0f;
        spriteRenderer.flipX = !playerIsRightOfFacing;
    }

    // Visual debugging: show where this object (or rotator) is facing and where the player is looking
    private void OnDrawGizmos()
    {
        // Choose the transform used for facing visualization
        Transform facing = (rotator != null) ? rotator : transform;

        // Draw facing direction
        Vector3 faceStart = facing.position;
        Vector3 faceDir = facing.forward;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(faceStart, faceStart + faceDir.normalized * 1.5f);
        Gizmos.DrawSphere(faceStart + faceDir.normalized * 1.5f, 0.03f);

        if (playerTransform != null)
        {
            // Draw player's look direction
            Vector3 playerPos = playerTransform.position;
            Vector3 playerDir = playerTransform.forward;
            playerDir.y = 0f;
            if (playerDir.sqrMagnitude > 1e-6f)
            {
                playerDir.Normalize();
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(playerPos, playerPos + playerDir * 1.5f);
                Gizmos.DrawSphere(playerPos + playerDir * 1.5f, 0.03f);
            }

            // Line between player and enemy
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerPos, transform.position);

            // Draw a small marker at the evaluated target (player + offset)
            Vector3 targ = playerPos + offset;
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.8f);
            Gizmos.DrawWireSphere(targ, 0.05f);

            // Draw the determined rotation for THIS gameObject (yaw toward player+offset + yaw offset)
            Vector3 dirToTarget = targ - transform.position;
            dirToTarget.y = 0f;
            if (dirToTarget.sqrMagnitude > 1e-6f)
            {
                float yaw = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
                yaw += rotatorXawOffsetDeg; // apply configured yaw offset
                Quaternion desired = Quaternion.Euler(0f, yaw, 0f);
                Vector3 desiredDir = desired * Vector3.forward;
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, transform.position + desiredDir.normalized * 1.25f);
                Gizmos.DrawSphere(transform.position + desiredDir.normalized * 1.25f, 0.03f);
            }
        }
    }

    private void RotateRotatorToFacePlayer()
    {
        if (playerTransform == null || rotator == null) return;

        // include positional offset when aiming
        Vector3 target = playerTransform.position + offset;
        Vector3 dir = target - rotator.position;

        if (dir.sqrMagnitude <= 1e-6f) return;

        // Yaw-only: project to horizontal plane
        Vector3 dirYaw = new Vector3(dir.x, 0f, dir.z);
        if (dirYaw.sqrMagnitude <= 1e-6f) return;

        float yaw = Mathf.Atan2(dirYaw.x, dirYaw.z) * Mathf.Rad2Deg;
        yaw += rotatorXawOffsetDeg;

        Vector3 currentEuler = rotator.rotation.eulerAngles;
        Quaternion desired = Quaternion.Euler(currentEuler.x, yaw-90, currentEuler.z);

        float alpha = 1f - Mathf.Exp(-rotatorLerpSpeed * Time.deltaTime);
        rotator.rotation = Quaternion.Slerp(rotator.rotation, desired, alpha);
    }
}