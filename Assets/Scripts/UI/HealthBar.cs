using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private Health health;

    public void BindHealthUI(Health health)
    {
        if (this.health != null) this.health.OnHealthChanged -= UpdateUI;
        this.health = health;
        this.health.OnHealthChanged += UpdateUI;
    }
        
    private void OnDestroy()
    {
        if (health != null) health.OnHealthChanged -= UpdateUI; 
    }
    private void UpdateUI(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }

}
