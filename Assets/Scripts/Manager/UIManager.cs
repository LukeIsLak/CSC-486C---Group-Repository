using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private HandViewUI handView;
    [SerializeField] private InventoryUI inventoryView;
    [SerializeField] private PauseUI pauseView;
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private StaminaUI staminaView;
    [SerializeField] private SetPlayerInputScheme inputScheme;
    [SerializeField] private TutorialUI tutorialView;
    
    [SerializeField] private DamageOverlayUI damageOverlay;

    // [SerializeField] private GameObject merchantPanel;
    [SerializeField] private GameObject interactPanel;
    // [SerializeField] private GameObject nodePanel;
    [SerializeField] private ChestUI chestUI;
    [SerializeField] private ShieldUI shieldUI;
    [SerializeField] private Animator swordAnimator;
    [SerializeField] private SwordAnimationEvent swordAnimationEvent;

    [SerializeField] private GameObject modifierEffects;
    public bool isInventoryOpen {  get; private set; }

    private PlayerInput playerInput;
    public static UIManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        if (healthBar == null) healthBar = GetComponentInChildren<HealthBar>(true);
        if(handView == null) handView = GetComponentInChildren<HandViewUI>(true);
        if(inventoryView == null) inventoryView = GetComponentInChildren<InventoryUI>(true);
        if(pauseView == null) pauseView = GetComponentInChildren<PauseUI>(true);
        if (staminaView == null) staminaView = GetComponentInChildren<StaminaUI>(true);
        if (chestUI == null) chestUI = GetComponentInChildren<ChestUI>(true);
        if (shieldUI == null) shieldUI = GetComponentInChildren<ShieldUI>(true);
        if (damageOverlay == null) damageOverlay = GetComponentInChildren<DamageOverlayUI>(true);
        HideInventoryView();
        HideCombatView();
        HidePauseView();
        HideMerchantView();
        HideInteract();
        HideNodePanel();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (Transform child in modifierEffects.transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null) img.enabled = false;
        }
    }

    public void BindPlayer(GameObject player)
    {
        var health = player.GetComponent<Health>();
        if (health != null)
        {   
            shieldUI.BindHealthUI(health);
            healthBar.BindHealthUI(health);
            damageOverlay.BindHealthUI(health);
        }

        var playerCharacter = player.GetComponent<PlayerCharacter>();
        if (playerCharacter != null) 
        {   
            staminaView.BindPlayerUI(playerCharacter);
            playerCharacter.SetSwordAnimator(swordAnimator);
            swordAnimationEvent.BindPlayer(playerCharacter);
        }
        playerInput = player.GetComponent<PlayerInput>();
        var deck = player.GetComponent<DeckSystems>();
        if (deck != null)
        {
            handView.BindDeckSystem(deck);  
            inventoryView.BindDeckSystem(deck);
        }
    }
    public void ShowCombatView()
    {
        combatPanel.SetActive(true);
        inputScheme.SetInputToCombat();
        
    }

    public void HideCombatView()
    {
        combatPanel.SetActive(false);
    }
    public void ShowInventoryView() 
    {
        isInventoryOpen = true;
        // HideCombatView();
        inputScheme.SetInputToInteractableUI();
        inventoryView.ShowInventory(); 
    }
    public void HideInventoryView()
    {
        isInventoryOpen = false;
        inventoryView.HideInventory();
        inputScheme.SetInputToCombat();
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

    public void ShowMerchantView()
    {
        // merchantPanel.SetActive(true);
    }

    public void HideMerchantView()
    {
        // merchantPanel.SetActive(false);
    }

    public void ShowInteract()
    {
        interactPanel.SetActive(true);
    }

    public void HideInteract()
    {
        interactPanel.SetActive(false);
    }

    public void ShowNodePanel()
    {
        // nodePanel.SetActive(true);
    }
    public void HideNodePanel()
    {
        // nodePanel.SetActive(false);
    }

    public void ShowChestUI(Acquirable cardData)
    {
        chestUI.ShowUI(cardData);
    }

    public void ShowTutorialUI(){
        tutorialView.showTutorial();
    }
}
    