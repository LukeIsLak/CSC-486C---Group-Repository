using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [Header("Events")]
    public GameEvent PlayerDeath;

    [Header("Data")]
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private CharacterData playerData;
    public float maxHealth;
    public float currentHealth;
    public bool hasDied = false;

    public int shield = 0;

    public event Action<float, float> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    
    private void NotifyHealthChanged() => OnHealthChanged?.Invoke(currentHealth, maxHealth);
    private void NotifyShieldChanged() => OnShieldChanged?.Invoke(shield);
    public void Init(float maxHealth, float currentHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        NotifyHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        //one of the cards gives the player a sheild that will block attacks, this will check if there is a shield active
        if (shield > 0) {
            shield--;
            NotifyShieldChanged();
            return;
        }

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
    public void AddShield(int amount)
    {
        shield += amount;
        NotifyShieldChanged();
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;
        Debug.Log(amount);
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        NotifyHealthChanged();
    }
    private void Die()
    {
        if (hasDied) return;
        hasDied = true;
        //Destroy(gameObject);
        PlayerInput pi = gameObject.GetComponent<PlayerInput>();
        pi.SwitchCurrentActionMap("UI");
        PlayerDeath.Raise();

    }

    public void UpdateCurrentHealthSO()
    {
        playerData.currentHealth = this.currentHealth;   
    }

    public void UpdateCurrentHealth(float amount)
    {
        this.currentHealth = amount;
        NotifyHealthChanged();
    }
}
