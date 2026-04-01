using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    [SerializeField] private PlayerCharacter character;
    HashSet<GameObject> hits = new HashSet<GameObject>();

    private Collider collider;
    bool canHit = true;
    private void Awake()
    {
        character = GetComponentInParent<PlayerCharacter>();
        collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;
        Debug.Log(other.tag);
        if (!other.CompareTag("Enemy")) return;

        if (hits.Contains(other.gameObject)) return;

        hits.Add(other.gameObject);
        var enemyComponent = other.GetComponentInParent<EnemyInterface>();
        Debug.Log(other.gameObject.name);
        if (enemyComponent != null) { 
            enemyComponent.GetComponent<FMODUnity.StudioEventEmitter>().Play(); 
            enemyComponent.Hit(character.attackMultiplier * character.GetAttackDamage()); 
            Debug.Log("EnemyHit"); 
        }
        //TriggerHitStop(attackHitStopDuration);

    }

    public void ResetHit()
    {
        hits.Clear();
    }

    public void EnableCollider()
    {
        canHit = true;
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        canHit = false;
        collider.enabled = false;
    }
}
