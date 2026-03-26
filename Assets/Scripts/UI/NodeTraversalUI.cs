using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeTraversalUI : MonoBehaviour
{
    [SerializeField] private RectTransform playerDeckLocation;
    [SerializeField] private RectTransform bufferLocation;
    [SerializeField] private RectTransform sideBoardLocation;
    [SerializeField] private GameObject inventoryInNode;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private ScrollRect playerDeckScrollRect; // For forcing showing item from the top
    [SerializeField] private ScrollRect bufferDeckScrollRect; // For forcing showing item from the top
    //[SerializeField] private GameObject openDeckButton;
    [SerializeField] private Button removeCardButton;
    [SerializeField] private TextMeshProUGUI tokenAmountUI;
    private List<CardViewUI> playerDeckCards = new();
    private List<CardViewUI> bufferCards = new();

    public GameEvent ToggleNodeInventory;

    private bool isRemoveMode = false;

    private void Start()
    {
        removeCardButton.onClick.AddListener(ToggleRemoveMode);
    }

    void OnEnable()
    {
        ShowDeckContainer();
    }

    void OnDisable()
    {
        HideDeckContainer();
    }
    public void ShowDeckContainer()
    {
        // openDeckButton.SetActive(false);
        inventoryInNode.SetActive(true);
        removeCardButton.interactable = playerInventory.amountRemovalTokens > 0;
        BuildDeckUI();
        BuildBufferUI();
        UpdateRemovalTokenUI();
        Canvas.ForceUpdateCanvases();
        playerDeckScrollRect.verticalNormalizedPosition = 1f; // top
        bufferDeckScrollRect.verticalNormalizedPosition = 1f;
        ToggleNodeInventory.Raise();
    }

    public void HideDeckContainer()
    {
        inventoryInNode.SetActive(false);
        //openDeckButton.SetActive(true);
        ClearDeckUI();
        ToggleNodeInventory.Raise();
    }
    private void ToggleRemoveMode()
    {
        if (playerInventory.amountRemovalTokens <= 0) return;
        isRemoveMode = !isRemoveMode;
        RefreshUI();
            
    }
    private void BuildDeckUI()
    {
        // Create the UI for each card instance in hand 
        foreach (var instance in playerInventory.playerdeck)
        {

            CardViewUI card = Instantiate(cardViewPrefab, playerDeckLocation);
            if(isRemoveMode)
            {
                card.Init(instance.cardData, !instance.useable, () =>
                {
                    playerInventory.removeCardFromPlayersDeck(instance);
                    isRemoveMode = false;
                    RefreshUI();
                    playerInventory.amountRemovalTokens--;
                    UpdateRemovalTokenUI();
                }, false); 
            }
            else
            {
                card.Init(instance.cardData, !instance.useable, null, true);
            }
                playerDeckCards.Add(card);
        }

    }
    private void UpdateRemovalTokenUI()
    {
        tokenAmountUI.text = playerInventory.amountRemovalTokens.ToString();
    }

    private void BuildBufferUI()
    {
        foreach (var instance in playerInventory.buffer)
        {
            CardViewUI card = Instantiate(cardViewPrefab, bufferLocation);

            card.Init(instance.cardData, !instance.useable, ()=> 
            {
                playerInventory.bufferToDeck(instance);
                RefreshUI();
            });
            bufferCards.Add(card);
        }
        Debug.Log(bufferLocation.offsetMax);
        Debug.Log(bufferLocation.offsetMin);
    }

    private void RefreshUI()
    {
        ClearDeckUI();
        BuildDeckUI();
        BuildBufferUI();
    }
    private void ClearDeckUI()
    {
        // Destroy all existing UI card gameObjects.
        foreach (var card in playerDeckCards)
        {
            Destroy(card.gameObject);
        }
        playerDeckCards.Clear();

        foreach (var card in bufferCards)
        {
            Destroy(card.gameObject);
        }
        bufferCards.Clear();
    }

    private void BuildSideBoardUI() {
        foreach (var instance in playerInventory.sideboard) {
            CardViewUI card = Instantiate(cardViewPrefab, sideBoardLocation);

            //card.Init(instance.cardData, !instance.useable, )
        }
    }
}
