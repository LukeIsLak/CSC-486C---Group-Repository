using UnityEngine;

public class EnemySpriteManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public float rotatorXawOffsetDeg;
    public float rotatorLerpSpeed = 1;

    void Update() {
        RotateToFacePlayer();
    }
    private void RotateToFacePlayer()
    {
        if (playerTransform == null) return;
        Vector3 target = playerTransform.position;
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        yaw += rotatorXawOffsetDeg;

        Quaternion desired = Quaternion.Euler(0f, yaw, 0f);

        float alpha = 1f - Mathf.Exp(-rotatorLerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, alpha);
    }
}