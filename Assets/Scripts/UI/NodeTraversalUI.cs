using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeTraversalUI : MonoBehaviour
{
    [SerializeField] private RectTransform containerLocation;
    [SerializeField] private GameObject inventoryInNode;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private ScrollRect scrollRect; // For forcing showing item from the top
    [SerializeField] private GameObject openDeckButton;
    private List<CardViewUI> cards = new();

    public void ShowDeckContainer()
    {
        openDeckButton.SetActive(false);
        inventoryInNode.SetActive(true);
        BuildDeckUI();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f; // top
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

            CardViewUI card = Instantiate(cardViewPrefab, containerLocation);
            
            card.Init(instance.cardData, !instance.useable);
            cards.Add(card);
        }

    }
    private void ClearDeckUI()
    {
        // Destroy all existing UI card gameObjects.
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }
}
