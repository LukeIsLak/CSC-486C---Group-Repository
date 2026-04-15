using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public GameEvent ExitToMainMenuEvent;

    public bool buttonClicked = false;
    public void QuitGamePressed()
    {
        if (buttonClicked) return;
        buttonClicked = true;
        Application.Quit();
    }

    public void ExitToMenuPressed()
    {
        if (buttonClicked) return;
        buttonClicked = true;
        ExitToMainMenuEvent.Raise();
    }
}
