using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SetPlayerInputScheme inputSchemeSetter;
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CardsDatabase cardsDB;
    public static GameManager instance { get; private set; }
    public PlayerInventory inventory;

    public bool tutorialWanted = false;

    private bool playerInitialized = false;
    private bool inventoryInitialized = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializedPlayerOnce(GameObject player)
    {
        if (playerInitialized) return;
        playerInitialized = true;
        
        var deck = player.GetComponent<DeckSystems>();
        //deck.InitializeRandomDeck(cardsDB, 30);
        //Debug.Log("I try thing");
    }
    private void InitializedDeckOnce()
    {
        return;
        if(inventoryInitialized) return;
        inventoryInitialized = true;
        //inventory.InitializeRandomDeck(cardsDB, 15);
        //inventory.Initialize();
    }
    private void InitializedPerScene(GameObject player)
    {
        UIManager.instance.BindPlayer(player);
        PauseManager.instance.BindPlayer(player);
        var playerHealth = player.GetComponent<Health>();
        playerHealth.Init(playerData.maxHealth, playerData.currentHealth);
        playerHealth.UpdateCurrentHealth(playerData.currentHealth);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneContext context = FindObjectOfType<SceneContext>();
        if(context == null)
        {
            Debug.LogWarning("No SceneType in scene");
            HideUI();
            return;

        }
        //if (context.doRefresh) inventory.refreshCards();
        switch (context.sceneType)
        {
            case SceneType.NodeTraversal:
                HandleNodeTraversalScene();
                break;
            case SceneType.Combat:
                HandleCombatScene();
                break;
            case SceneType.Merchant:
                HandleMerchantScene();
                break;
            case SceneType.DoNothing:
                HideUI();
                break;
            case SceneType.ForceMouseOn:
                HideUI();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }
    private void HideUI()
    {
        UIManager.instance.HideCombatView();
        UIManager.instance.HideMerchantView();
        UIManager.instance.HideNodePanel();
    }
    private void HandleNodeTraversalScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InitializedDeckOnce();
        UIManager.instance.ShowNodePanel();
        UIManager.instance.HideCombatView();
        UIManager.instance.HideMerchantView();

    }

    private void HandleCombatScene()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogError("no player in the scene");
            return;
        }
        inputSchemeSetter.SetAllOff();
        InitializedPlayerOnce(player);
        InitializedPerScene(player);
        UIManager.instance.ShowCombatView();
        UIManager.instance.HideNodePanel();
        UIManager.instance.HideMerchantView();
        if(tutorialWanted){
            Debug.Log("working");
            UIManager.instance.ShowTutorialUI();
            inputSchemeSetter.SetInputToInteractableUI();
            tutorialWanted =  false;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputSchemeSetter.SetInputToCombat();
        }
    }
    private void HandleMerchantScene() 
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InitializedDeckOnce(); // this will be remove when the game is in placed
        UIManager.instance.ShowMerchantView();
        UIManager.instance.HideCombatView();
        UIManager.instance.HideNodePanel();
    }

    //flips the tutorialWanted variable, triggers if the player changes the state of the check box on the title screen
    public void onTutoialSelected(){
        tutorialWanted = !tutorialWanted;
    }
}
