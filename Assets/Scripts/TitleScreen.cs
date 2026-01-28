using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{

    public void OnPlayButton(){
        // On pressing play, load intro level
        SceneManager.LoadScene(1);
    }

    //public void OnOptionsButton(){
        //Implement this when there is an options menu created
   // }

    public void OnQuitButton(){
        Application.Quit();
    }

}
