using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameEvent StartGame;

    private bool startPressed = false;
    void Start() {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);

        FMOD.Studio.Bus masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

    }

    public void OnPlayButton(){
        if (startPressed) return;
        startPressed = true;
        // On pressing play, load intro level
        StartGame.Raise();
    }

    public void OnQuitButton(){
        if (startPressed) return;
        // On pressing quit, quit the application
        Application.Quit();
    }

}
