using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryLocation;
    [SerializeField] private CardViewUI cardViewPrefab;
    [SerializeField] private ScrollRect scrollRect; // For forcing showing item from the top
    private List<CardViewUI> cards = new();
    private DeckSystems deckSystems;

    public void BindDeckSystem(DeckSystems d)
    {
        if (deckSystems == d) return; 
        deckSystems = d;
    }
    private void BuildInventoryUI()
    {
        // Create the UI for each card instance in hand 
        foreach (var instance in deckSystems.deck)
        {

            CardViewUI card = Instantiate(cardViewPrefab, inventoryLocation);
            card.Init(instance.cardData);
            cards.Add(card);
        }

    }
    private void ClearInventoryUI()
    {
        // Destroy all existing UI card gameObjects.
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }
    public void ShowInventory()
    {
        gameObject.SetActive(true);
        BuildInventoryUI();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f; // top
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideInventory()
    {
        ClearInventoryUI();
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
