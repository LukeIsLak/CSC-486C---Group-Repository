using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSystem : MonoBehaviour
{
    /* Use this for intercommunication of systems.
    The idea is simple: create an event here and you 
    can subscribe to it in your script's Start().

    Then, the event can be invoked from anywhere else,
    and the function provided in the listener will be run.
    */
    public UnityEvent           ExitLobbyToLayout,
                                ExitLayoutToEncounter,
                                ExitEncounterToLayout,
                                ExitToMainMenu;

    void Start()
    {
        ExitLobbyToLayout       = new UnityEvent();
        ExitLayoutToEncounter   = new UnityEvent();
        ExitEncounterToLayout   = new UnityEvent();
        ExitToMainMenu          = new UnityEvent();
    }
}
