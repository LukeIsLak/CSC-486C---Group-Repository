using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 start;
    public CardViewUI cardUI;
    public CardInstance cardInstance;

    void Start()
    {
        cardInstance = cardUI.cardInstance;
    }

    public Deck activeDeck;
    public Deck bufferDeck;
    public Deck sideboardDeck;

    public void OnBeginDrag(PointerEventData eventData) {
        if (cardInstance == null || !cardInstance.deck) return;
        if (cardInstance.deck == activeDeck) return;
        //start = transform.position;
        DragHandler.instance.HandleDragStart(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        if (cardInstance == null || !cardInstance.deck) return;

        if (cardInstance.deck == activeDeck) return;
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (cardInstance == null || !cardInstance.deck) return;
        if (cardInstance.deck == activeDeck) return;
        DragHandler.instance.HandleDragEnd(gameObject, cardInstance);
        //if (transform.position.x > -50)
        //transform.position = start;
        
    }
}
