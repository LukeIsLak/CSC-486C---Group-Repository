using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Deck associatedDeck;
    public Transform associatedTransform;
    public void OnPointerEnter(PointerEventData pointerEventData) 
    {
        DragHandler.instance.SetHoveredDeck(associatedDeck, associatedTransform);
    }

    public void OnPointerExit(PointerEventData pointerEventData) 
    {
        DragHandler.instance.UnsetHoveredDeck(associatedDeck);
    }
}
