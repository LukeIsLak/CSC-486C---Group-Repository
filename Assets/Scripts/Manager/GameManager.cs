using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private CharacterData playerData;
    [SerializeField] private CardsDatabase cardsDB;
    public static GameManager instance { get; private set; }


    private Health playerhealth;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject player = Instantiate(playerPrefab);


        var deck = player.GetComponent<DeckSystems>();
        var playerHealth = player.GetComponent<Health>();

        playerHealth.Init(playerData.maxHealth);
        deck.InitializeRandomDeck(cardsDB, 30); 
        UIManager.instance.BindPlayer(player);
    }

    private void Update()
    {
        // for testing player health
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerhealth.TakeDamage(10);
        }
    }
}
