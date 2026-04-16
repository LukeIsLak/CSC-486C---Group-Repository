using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool isPause = false;
    private PlayerInput playerInput;    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
        FMODUnity.RuntimeManager.PauseAllEvents(true);

        if (playerInput != null) playerInput.SwitchCurrentActionMap("Menu UI");
        UIManager.instance.ShowPauseView();

    }

    public void Resume()
    {
        isPause = false;
        FMODUnity.RuntimeManager.PauseAllEvents(false);

        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerInput != null) playerInput.SwitchCurrentActionMap("Combat");
        UIManager.instance.HidePauseView();
    }
}
