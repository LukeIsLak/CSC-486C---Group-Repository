using System.Collections;
using UnityEngine;

public class DivineJudgementAttack : MonoBehaviour
{
    [Header("Setup")]
    public float warningDuration = 1.5f;
    public float lingerDuration = 1.0f;
    public float dmg = 30f;
    public float radius = 4f;
    public LayerMask playerLayer;
    public GameObject warningCirclePrefab;
    public GameObject warning;

    public CapsuleCollider hitbox;
    private bool canDamage = false;

    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, 100f, LayerMask.GetMask("Surface")))
        {
            transform.position = hit.point;
        }

        if (warningCirclePrefab)
        {
            GameObject circle = Instantiate(warningCirclePrefab, transform.position, Quaternion.identity, transform);
            warning = circle;
        }

        hitbox.enabled = false;

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(warningDuration);
        hitbox.enabled = true;
        canDamage = true;
        yield return new WaitForSeconds(lingerDuration);
        Destroy(warning);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;
        if (other.CompareTag("Player") && playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            Health h = other.GetComponent<Health>();
            if (h != null) h.TakeDamage(dmg);
        }
    }
}