using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private HandViewUI handView;
    [SerializeField] private InventoryUI inventoryView;
    [SerializeField] private GameObject CombatPanel;

    private PlayerInput playerInput;
    public static UIManager instance { get; private set; }


    private void Awake()
    {
        instance = this;
    }
    public void BindPlayer(GameObject player)
    {
        var health = player.GetComponent<Health>();
        if(health != null ) healthBar.BindHealthUI(health);
        playerInput = player.GetComponent<PlayerInput>();
        var deck = player.GetComponent<DeckSystems>();
        if (deck != null)
        {
            handView.BindDeckSystem(deck);
            inventoryView.BindDeckSystem(deck);
        }
    }
    private void ShowCombatView()
    {
        CombatPanel.SetActive(true);
    }

    private void HideCombatView()
    {
        CombatPanel.SetActive(false);
    }
    public void ShowInventoryView() 
    {
        HideCombatView();
        playerInput.SwitchCurrentActionMap("UI");
        inventoryView.ShowInventory(); 
    }
    public void HideInventoryView()
    {
        inventoryView.HideInventory();
        playerInput.SwitchCurrentActionMap("Combat");
        ShowCombatView();
    }


}
