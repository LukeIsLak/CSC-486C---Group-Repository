using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrambleGrenade : MonoBehaviour
{
    public BrambleTrap trapPrefab;

    [SerializeField] private float forwardForce = 5f;
    [SerializeField] private float upwardForce = 5f;


    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Throw(Vector3 direction)
    {
        //Throw the projectile in grenade like arc
        Vector3 arc = direction.normalized * forwardForce + Vector3.up * upwardForce;

        rb.AddForce(arc, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        //When it lands, spawn the trap at that point
        Vector3 hitPoint = other.contacts[0].point;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal);
        Instantiate(trapPrefab, hitPoint + Vector3.up * 0.02f, rotation);
        Destroy(this.gameObject);
    }
}
