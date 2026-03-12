using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    //Need to implement:
        //SFX slider
        //BGM slider
        

    public GameObject Options;
    public GameObject MainOptions;
    public GameObject ControlsMenu;
    public GameObject TitleScreen = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    /*
    void Update()
    {
        //If escape pressed down ever, Activate the Options menu
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != "TitleScreen"){
            if (Input.GetKeyDown(KeyCode.Escape)){
            EnableOptions();
        }
        }
        
    }
    */

    public void EnableOptions(){
        // Turn on the options and pause time
        Options.SetActive(true);
        MainOptions.SetActive(true);
        // Time.timeScale = 0f;

    }

    public void HideAll(){
        //Hide the menus
        MainOptions.SetActive(false);
        ControlsMenu.SetActive(false);
        Options.SetActive(false);
    }


    private void DisableOptions(){
        //Disable the menus and resume gameplay
        HideAll();
        // Time.timeScale = 1f;
        string sceneName = SceneManager.GetActiveScene().name;

        if (TitleScreen != null)
        TitleScreen.SetActive(true);

    }

    public void OnExitButton(){
        // If exit is clicked then leave the options menu
        DisableOptions();

    }

    public void OnOptionsButton(){
        // On pressing options, switch ui to show the options menu and not the
        if (TitleScreen != null)
        TitleScreen.SetActive(false);
        EnableOptions();
    }

}
