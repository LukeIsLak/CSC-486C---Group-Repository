using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonRangedBone : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;
    public float damage = 10f;
    private Rigidbody rb;

    public void LaunchArc(Vector3 target, float arcPeakHeight = 2f) {
        rb = GetComponent<Rigidbody>();
        Vector3 start = transform.position;
        Vector3 displacement = target - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float distanceXZ = displacementXZ.magnitude;
        float totalTime = distanceXZ / speed;
        float gravity = Mathf.Abs(Physics.gravity.y);
        float heightDifference = target.y - start.y;
        float peak = Mathf.Max(start.y, target.y) + arcPeakHeight;

        float timeToPeak = Mathf.Sqrt(2 * (peak - start.y) / gravity);
        float timeFromPeak = Mathf.Sqrt(2 * (peak - target.y) / gravity);
        float arcTime = timeToPeak + timeFromPeak;

        float t = Mathf.Max(totalTime, arcTime);
        Vector3 velocityXZ = displacementXZ / t;
        float velocityY = (heightDifference + 0.5f * gravity * t * t) / t;
        Vector3 velocity = velocityXZ + Vector3.up * velocityY;
        rb.velocity = velocity;

        Destroy(gameObject, lifeTime);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Health h = other.gameObject.GetComponent<Health>();
            if (h != null) h.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}