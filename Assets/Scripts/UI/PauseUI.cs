using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public void ShowPauseUI()
    {
        gameObject.SetActive(true);
    }

    public void HidePauseUI()
    {
        gameObject.SetActive(false);
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
        // show option panel
    }

}
