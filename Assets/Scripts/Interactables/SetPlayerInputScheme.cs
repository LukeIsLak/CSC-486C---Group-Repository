using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetPlayerInputScheme : MonoBehaviour
{
    private PlayerInput playerInput;

    private bool BindPlayerInput()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (!player) { Debug.LogWarning("No player found in scene"); return false; }
        playerInput = player.GetComponent<PlayerInput>();
        if (!playerInput) { Debug.LogWarning("No PlayerInput on player"); return false; }
        return true;
    }

    public void SetAllOff()
    {
        if (!BindPlayerInput()) return;
        foreach (var map in playerInput.actions.actionMaps) 
        {
            //if (map.name != "UI") 
            map.Disable();
        }

    }
    
    public void SetInputToInteractableUI()
    {
        if (!BindPlayerInput()) return;
        playerInput.SwitchCurrentActionMap("Interactable UI");
    }
    public void SetInputToCombat()
    {
        if (!BindPlayerInput()) return;
        playerInput.SwitchCurrentActionMap("Combat");
    }
}
