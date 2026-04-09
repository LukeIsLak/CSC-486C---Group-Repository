using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public List<GameObject> tutorialSlides;
    public List<GameObject> buttons;
    public GameObject panel;
    private int index = 0;
    public SetPlayerInputScheme inputSchemeSetter;
    


    public void showTutorial(){
        tutorialSlides[index].SetActive(true);
        panel.SetActive(true);
    }

    public void onNextClicked(){
        tutorialSlides[index].SetActive(false);

        if (index + 1 > tutorialSlides.Count){
            index = tutorialSlides.Count;
        } else {
            index++;
        }

        tutorialSlides[index].SetActive(true);
    }

    public void onBackClicked(){
        tutorialSlides[index].SetActive(false);

        if (index - 1 < 0){
            index = 0;
        } else {
            index--;
        }

        tutorialSlides[index].SetActive(true);
    }

    public void onExitClicked(){
        tutorialSlides[index].SetActive(false);
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputSchemeSetter.SetInputToCombat();
    }
}
