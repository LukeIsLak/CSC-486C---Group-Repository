using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeTraversalUI : MonoBehaviour
{
    [SerializeField] private RectTransform playerDeckLocation;
    [SerializeField] private RectTransform bufferLocation;
    [SerializeField] private GameObject inventoryInNode;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private ScrollRect playerDeckScrollRect; // For forcing showing item from the top
    [SerializeField] private ScrollRect bufferDeckScrollRect; // For forcing showing item from the top
    [SerializeField] private GameObject openDeckButton;
    private List<CardViewUI> playerDeckCards = new();
    private List<CardViewUI> bufferCards = new();
    

    public void ShowDeckContainer()
    {
        openDeckButton.SetActive(false);
        inventoryInNode.SetActive(true);
        BuildDeckUI();
        BuildBufferUI();
        Canvas.ForceUpdateCanvases();
        playerDeckScrollRect.verticalNormalizedPosition = 1f; // top
        bufferDeckScrollRect.verticalNormalizedPosition = 1f;
    }

    public void HideDeckContainer()
    {
        inventoryInNode.SetActive(false);
        openDeckButton.SetActive(true);
        ClearDeckUI();
    }

    private void BuildDeckUI()
    {
        // Create the UI for each card instance in hand 
        foreach (var instance in playerInventory.playerdeck)
        {

            CardViewUI card = Instantiate(cardViewPrefab, playerDeckLocation);
            
            card.Init(instance.cardData, !instance.useable);
            playerDeckCards.Add(card);
        }

    }

    private void BuildBufferUI()
    {
        foreach (var instance in playerInventory.buffer)
        {
            CardViewUI card = Instantiate(cardViewPrefab, bufferLocation);

            card.Init(instance.cardData, !instance.useable);
            bufferCards.Add(card);
        }
    }

    private void OnBufferCardClicked(CardInstance card)
    {
        playerInventory.bufferToDeck(card);

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
}
