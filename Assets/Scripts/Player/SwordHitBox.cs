using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitBox : MonoBehaviour
{
    [SerializeField] private PlayerCharacter character;
    HashSet<GameObject> hits = new HashSet<GameObject>();

    bool canHit;
    private void Awake()
    {
        character = GetComponent<PlayerCharacter>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;    
        if (!other.CompareTag("Enemy")) return;

        if(hits.Contains(other.gameObject)) return;

        hits.Add(other.gameObject);

        var enemyComponent = other.GetComponentInParent<EnemyInterface>();
        if (enemyComponent != null) enemyComponent.GetComponent<FMODUnity.StudioEventEmitter>().Play(); enemyComponent.Hit(character.attackMultiplier * character.GetAttackDamage());
        //TriggerHitStop(attackHitStopDuration);

    }

    public void ResetHit()
    {
        hits.Clear();
    }

    public void EnableCollider()
    {
        canHit = true;
    }

    public void DisableCollider()
    {
        canHit = false;
    }
}
