using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameEvent StartGame;

    void Start() {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);

    }

    public void OnPlayButton(){
        // On pressing play, load intro level
        StartGame.Raise();
    }

    public void OnQuitButton(){
        // On pressing quit, quit the application
        Application.Quit();
    }

}
