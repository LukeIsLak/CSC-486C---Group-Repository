using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    [SerializeField] private float delay = 0.5f;

    private Coroutine TooltipRoutine;
    public void SetContent(string header, string content)
    {
        this.header = header;
        this.content = content;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover enter on " + gameObject.name);
        TooltipRoutine = StartCoroutine(ShowTooltipDelay());
    }

    public void OnPointerExit(PointerEventData eventData) 
    {   
        if(TooltipRoutine != null) StopCoroutine(TooltipRoutine);
        
        TooltipUI.instance?.Hide();
    }

    private IEnumerator ShowTooltipDelay()
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Tooltip instance is: " + TooltipUI.instance);
        TooltipUI.instance?.Show(header, content);
    }
}
