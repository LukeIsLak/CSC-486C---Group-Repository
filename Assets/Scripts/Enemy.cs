using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Health health;
    [SerializeField] private CharacterData enemyData;

    private void Awake()
    {
        health = GetComponent<Health>();
        health.Init(enemyData.maxHealth);
    }
}
