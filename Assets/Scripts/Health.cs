using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    [Header("Events")]
    public GameEvent ExitToLayout;

    [Header("Data")]
    [SerializeField] private HealthBar healthBar;
    public float maxHealth { get; private set; }
    public float currentHealth {  get; private set; }

    public int shield = 0;

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

        //one of the cards gives the player a sheild that will block attacks, this will check if there is a shield active
        if (shield > 0) {
            shield--;
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

    public void Heal(float amount)
    {
        if (amount <= 0) return;
        Debug.Log(amount);
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        NotifyHealthChanged();
    }
    private void Die()
    {
        //Destroy(gameObject);
        PlayerInput pi = gameObject.GetComponent<PlayerInput>();
        pi.SwitchCurrentActionMap("UI");
        ExitToLayout.Raise();

    }


}
