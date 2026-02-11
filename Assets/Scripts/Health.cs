using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    public float maxHealth { get; private set; }
    public float currentHealth {  get; private set; }

   
    public void Init(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.SetMaxHealthUI(this.maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        if (healthBar != null) healthBar.SetHealthUI(currentHealth);
        Debug.Log(currentHealth);
        if (currentHealth < 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    private void Die()
    {
        Destroy(gameObject);
    }


}
