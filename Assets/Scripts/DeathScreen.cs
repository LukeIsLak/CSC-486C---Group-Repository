using UnityEngine.UI;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public GameEvent ExitToMainMenuEvent;

    public bool buttonClicked = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
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
