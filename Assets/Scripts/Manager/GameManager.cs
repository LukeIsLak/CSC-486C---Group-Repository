using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CardsDatabase cardsDB;
    public static GameManager instance { get; private set; }
    public PlayerInventory inventory;

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
        var playerHealth = player.GetComponent<Health>();

        playerHealth.Init(playerData.maxHealth);

        //deck.InitializeRandomDeck(cardsDB, 30);
        //Debug.Log("I try thing");
       
    }
    private void InitializedDeckOnce()
    {
        if(inventoryInitialized) return;
        inventoryInitialized = true;
        inventory.InitializeRandomDeck(cardsDB, 30);
    }
    private void InitializedPerScene(GameObject player)
    {
        UIManager.instance.BindPlayer(player);
        PauseManager.instance.BindPlayer(player);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneContext context = FindObjectOfType<SceneContext>();
        if(context == null)
        {
            Debug.LogError("No SceneType in scene");
        }
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
        }
    }

    private void HandleNodeTraversalScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InitializedDeckOnce();
        UIManager.instance.ShowNodePanel();
    }

    private void HandleCombatScene()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogError("no player in the scene");
            return;
        }
        InitializedPlayerOnce(player);
        InitializedPerScene(player);
        InitializedDeckOnce(); // this will be remove when the game is in placed
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.instance.ShowCombatView();
    }
    private void HandleMerchantScene() 
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InitializedDeckOnce(); // this will be remove when the game is in placed
        UIManager.instance.ShowMerchantView();
    }
}
