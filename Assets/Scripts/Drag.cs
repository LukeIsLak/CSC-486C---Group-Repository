using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
    Transform start;
    
    public void OnBeginDrag(PointerEventData eventData) {
        start = transform;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        //if (transform.position.x > -50)
    }

    private void findLocation(Transform location){
        //if (location.x > -175 && location.x <)
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        Debug.Log(name);
    }
}
