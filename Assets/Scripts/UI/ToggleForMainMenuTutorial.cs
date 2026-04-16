using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleForMainMenuTutorial : MonoBehaviour
{
    private GameObject gameObject;
    private GameManager gamemanager;

    public void OnToggle(){
        gameObject = GameObject.FindWithTag("gameManager");
        gamemanager = gameObject.GetComponent(typeof(GameManager)) as GameManager;
        gamemanager.onTutoialSelected();
    }
}
