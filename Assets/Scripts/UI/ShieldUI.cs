using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldUI : MonoBehaviour
{
    [SerializeField] private GameObject shieldUIPrefabs;
    [SerializeField] private RectTransform location;
    private Health health;
    private List<GameObject> shieldUIs = new List<GameObject>();
    public void BindHealthUI(Health health)
    {

        if (this.health != null)
        {

             this.health.OnShieldChanged -= RefreshShieldUI;
        }
        this.health = health;

        if (this.health != null)
        {

            this.health.OnShieldChanged += RefreshShieldUI;
            RefreshShieldUI(this.health.shield);
        }
    }
    private void OnDestroy()
    {
        if (this.health != null) health.OnShieldChanged -= RefreshShieldUI;
    }

    private void RefreshShieldUI(int currentShield)
    {
        ClearUI();

        for (int i = 0; i < currentShield; i++)
        {
            {
                GameObject shieldUI = Instantiate(shieldUIPrefabs, location);
                shieldUIs.Add(shieldUI);
            }
        }
    }

    private void ClearUI()
    {
        foreach (var shield in shieldUIs)
        {
            Destroy(shield.gameObject);

        }
        shieldUIs.Clear();
    }
}
