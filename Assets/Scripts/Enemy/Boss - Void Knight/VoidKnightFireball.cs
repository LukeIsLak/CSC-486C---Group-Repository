using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidKnightFireball : MonoBehaviour
{
    [SerializeField] private float projspeed = 10f;
    [SerializeField] private float dmgradius = 3f;
    [SerializeField] private float ttl = 5f;
    public LayerMask playerLayer;
    public float dmg;
    public Vector3 dir;    
    public void Initialize(Vector3 direction) {
        dir = direction.normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
        StartCoroutine(timeToLive(ttl));
    }

    // Update is called once per frame    
    void Update()
    {
        transform.position += dir * projspeed * Time.deltaTime;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    void OnCollisionEnter(Collision other) {
        Impact();
    }

    void Impact(){
        //Using projectile information, deal damage to all objects in the area

        Collider[] impactArea = Physics.OverlapSphere(transform.position, dmgradius, playerLayer);

        foreach (Collider hit in impactArea) {
            if (hit.CompareTag("Player")) {
                Health h = hit.GetComponent<Health>();
                if (h != null) h.TakeDamage(dmg);
            }
        }

        Destroy(this.gameObject);
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Impact();
    }
}
