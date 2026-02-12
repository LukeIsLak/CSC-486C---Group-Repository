using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    public float maxHealth { get; private set; }
    public float currentHealth {  get; private set; }

    public event Action<float, float> OnHealthChanged;
    
    private void NotifyHealthChanged() => OnHealthChanged?.Invoke(currentHealth, maxHealth);
    public void Init(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        Debug.Log(currentHealth);
        if (currentHealth < 0)
        {
            currentHealth = 0;
            NotifyHealthChanged();
            Die();
            return;
        }
        NotifyHealthChanged();
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        NotifyHealthChanged();
    }
    private void Die()
    {
        Destroy(gameObject);
    }


}
