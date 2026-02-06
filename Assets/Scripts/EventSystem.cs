using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSystem : MonoBehaviour
{
    public static EventSystem instance;

    /* Use this for intercommunication of systems.
    The idea is simple: create an event here and you 
    can subscribe to it in your script's Start().

    Then, the event can be invoked from anywhere else,
    and the function provided in the listener will be run.
    */

    /* Scene management */
    public UnityEvent ExitLobbyToLayout       = new UnityEvent();
    public UnityEvent ExitLayoutToEncounter   = new UnityEvent();
    public UnityEvent ExitEncounterToLayout   = new UnityEvent();
    public UnityEvent ExitToMainMenu          = new UnityEvent();


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
