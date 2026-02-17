using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool isPause {  get; private set; }
    private PlayerInput playerInput;
    private void Awake()
    {
        instance = this;
    }
    public void BindPlayer(GameObject player)
    {
        playerInput = player.GetComponent<PlayerInput>();
    }

    public void Pause()
    {
        isPause = true;
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInput.SwitchCurrentActionMap("UI");
        UIManager.instance.ShowPauseView();

    }

    public void Resume()
    {
        isPause = false;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.SwitchCurrentActionMap("Combat");
        UIManager.instance.HidePauseView();
    }
}
