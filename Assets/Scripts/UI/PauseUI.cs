using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public OptionsMenu optionsMenu;
    public GameEvent ExitToMainMenu;
    public void ShowPauseUI()
    {
        gameObject.SetActive(true);
    }

    public void HidePauseUI()
    {
        gameObject.SetActive(false);
        optionsMenu.HideAll();
    }

    public void OnResumeClicked()
    {
        PauseManager.instance.Resume();
    }

    public void OnQuitClicked()
    {
        // add load to menu scene
    }

    public void OnOptionsClicked()
    {
        optionsMenu.gameObject.SetActive(true);
        optionsMenu.EnableOptions();
    }

    public void OnExitClicked()
    {
        PauseManager.instance.Resume();
        ExitToMainMenu.Raise();
    }

}
