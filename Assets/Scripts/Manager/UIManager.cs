using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private HandViewUI handView;
    [SerializeField] private InventoryUI inventoryView;

    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    public void BindPlayer(GameObject player)
    {
        var health = player.GetComponent<Health>();
        if(health != null ) healthBar.BindHealthUI(health);

        var deck = player.GetComponent<DeckSystems>();
        if (deck != null)
        {
            handView.BindDeckSystem(deck);
            inventoryView.BindDeckSystem(deck);
        }
    }
}
