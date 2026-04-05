using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI healthText;
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
        if (healthText != null) healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

}
