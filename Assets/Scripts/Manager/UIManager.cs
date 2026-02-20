using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private HandViewUI handView;
    [SerializeField] private InventoryUI inventoryView;
    [SerializeField] private PauseUI pauseView;
    [SerializeField] private GameObject CombatPanel;
    [SerializeField] private StaminaUI staminaView;
    public bool isInventoryOpen {  get; private set; }

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
        var playerCharacter = player.GetComponent<PlayerCharacter>();
        if(playerCharacter != null) staminaView.BindPlayerUI(playerCharacter);
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
        isInventoryOpen = true;
        HideCombatView();
        playerInput.SwitchCurrentActionMap("UI");
        inventoryView.ShowInventory(); 
    }
    public void HideInventoryView()
    {
        isInventoryOpen = false;
        inventoryView.HideInventory();
        playerInput.SwitchCurrentActionMap("Combat");
        ShowCombatView();
    }

    public void ShowPauseView()
    {
        pauseView.ShowPauseUI();
    }
    public void HidePauseView()
    {
        pauseView.HidePauseUI();
    }
}
