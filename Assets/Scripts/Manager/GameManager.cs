using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CardsDatabase cardsDB;
    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player NULL");
            return;
        }
        InitializedAll(player); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void InitializedAll(GameObject player)
    {
        var deck = player.GetComponent<DeckSystems>();
        var playerHealth = player.GetComponent<Health>();

        playerHealth.Init(playerData.maxHealth);

        deck.InitializeRandomDeck(cardsDB, 30);

        UIManager.instance.BindPlayer(player);
        UIManager.instance.HideInventoryView();
        UIManager.instance.HidePauseView();

        PauseManager.instance.BindPlayer(player);
    }
}
