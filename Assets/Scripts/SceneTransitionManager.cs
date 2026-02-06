using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;

    private PersistentData pd;
    private EventSystem es;


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


    void Start()
    {
        // Subscribe to appropriate unity events
        pd = GameObject.FindWithTag("Persistent Data").GetComponent<PersistentData>();
        es = GameObject.FindWithTag("Event System").GetComponent<EventSystem>();
        if (es) 
        {
            es.ExitLobbyToLayout.AddListener(SceneSwapToMapLayoutStart);
            es.ExitLayoutToEncounter.AddListener(SceneSwapToEncounter);
            es.ExitEncounterToLayout.AddListener(SceneSwapToMapLayoutProgress);
            es.ExitToMainMenu.AddListener(SceneSwapToMainMenu);
        }
    }


    /**********************************
    ************ Traversal ************
    **********************************/

    void SceneSwapToMapLayoutStart()
    {
        SceneManager.LoadScene("Scenes/LayoutTraversal");
    }

    void SceneSwapToEncounter()
    {
        EncounterType encType = pd.currentEncounterType;
    
        if (encType == EncounterType.None) 
        {
            Debug.Log("No encounter type provided.");
            return;
        }


        if (encType == EncounterType.Merchant)
        {
            SceneManager.LoadScene("Scenes/Encounters/Merchant");
        }
        if (encType == EncounterType.Treasure)
        {
            SceneManager.LoadScene("Scenes/Encounters/Treasure");
        }
        if (encType == EncounterType.Dungeon)
        {
            SceneManager.LoadScene("Scenes/Encounters/Dungeon");
        }
        if (encType == EncounterType.Boss)
        {
            SceneManager.LoadScene("Scenes/Encounters/Boss");  
        }
        else 
        {
            Debug.Log("Nothing to do for you with this encounter type.");
        }
    }

    void SceneSwapToMapLayoutProgress()
    {
        SceneManager.LoadScene("Scenes/LayoutTraversal");
    }

    void SceneSwapToMainMenu()
    {

    }
}
