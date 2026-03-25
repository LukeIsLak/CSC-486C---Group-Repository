using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangeInteractable : MonoBehaviour
{
    public string playerTag;
    public UnityEvent onInteract;
    public UnityEvent onEnterRange;
    public UnityEvent onLeaveRange;
    public bool isOneTime = true;
    public bool activated;
    private bool playerInRange;

    public GameObject graphicsWithinRange;

    public void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag(playerTag)) return;
        if (isOneTime && activated) return;
        TurnOn();
    }

    public void OnTriggerExit(Collider collider)
    {
        if (!collider.gameObject.CompareTag(playerTag)) return;
        TurnOff();
    }
    public void OnPlayerInteract()
    {
        // Don't allow if already activated or not in range
        if (isOneTime && activated) return;
        if (!playerInRange)         return;

        onInteract.Invoke();
        activated = !activated;

        // If one time use turn off usage graphics
        if (isOneTime) TurnOff();
    }

    private void TurnOn()
    {
        onEnterRange.Invoke();
        if (graphicsWithinRange != null) graphicsWithinRange.SetActive(true);
        playerInRange = true;
    }
    private void TurnOff()
    {
        onLeaveRange.Invoke();
        if (graphicsWithinRange != null) graphicsWithinRange.SetActive(false);
        playerInRange = false;
    }
}
