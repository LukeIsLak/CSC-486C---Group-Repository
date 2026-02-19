using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CardsDatabase cardsDB;
    public static GameManager instance { get; private set; }
    public PlayerInventory inventory;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameObject player = Instantiate(playerPrefab);

        //var deck = player.GetComponent<DeckSystems>();
        var playerHealth = player.GetComponent<Health>();

        playerHealth.Init(playerData.maxHealth);

        //deck.InitializeRandomDeck(cardsDB, 30);
        Debug.Log("I try thing");
        inventory.InitializeRandomDeck(cardsDB, 30);

        UIManager.instance.BindPlayer(player);
        UIManager.instance.HideInventoryView();
        UIManager.instance.HidePauseView();

        PauseManager.instance.BindPlayer(player);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
