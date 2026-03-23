using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInteractable : RangeInteractable
{
    public GameObject userInterface;
    public SetPlayerInputScheme inputSchemeSetter;
    public bool displaying;


    public void OpenUI()
    {
        displaying = true;
        userInterface.SetActive(true);
        inputSchemeSetter.SetInputToInteractableUI();

        Cursor.lockState    = CursorLockMode.None;
        Cursor.visible      = true;
    }
    public void CloseUI()
    {
        if (!displaying) return;
        userInterface.SetActive(false);
        displaying = false;
        inputSchemeSetter.SetInputToCombat();

        Cursor.lockState    = CursorLockMode.Locked;
        Cursor.visible      = false;
    }

}
