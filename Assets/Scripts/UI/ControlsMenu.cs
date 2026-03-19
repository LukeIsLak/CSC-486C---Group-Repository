using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public GameObject Options;
    public GameObject Controls;
    public GameObject MainOptions;
    // Start is called before the first frame update
    void Start()
    {
        // MainOptions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnExitButton(){
        MainOptions.SetActive(true);
        Controls.SetActive(false);
    }

    public void OnControlsButton(){
        MainOptions.SetActive(false);
        Controls.SetActive(true);
    }

}
