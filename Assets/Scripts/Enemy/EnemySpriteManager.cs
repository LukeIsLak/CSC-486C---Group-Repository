using UnityEngine;

public class EnemySpriteManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public float rotatorXawOffsetDeg;
    public float rotatorLerpSpeed = 1;

    void Start() {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update() {
        RotateToFacePlayer();
    }

    private void RotateToFacePlayer()
    {
        /*If the player is not found, ignore this function*/
        if (playerTransform == null) return;

        /*Find the direction of the player*/
        Vector3 target = playerTransform.position;
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        /*Angle of which to look at the player (plus a given offset)*/
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        yaw += rotatorXawOffsetDeg;

        /*Rotate towards the player with the new "yaw" angle*/
        Quaternion desired = Quaternion.Euler(0f, yaw, 0f);
        float alpha = 1f - Mathf.Exp(-rotatorLerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, alpha);
    }
}