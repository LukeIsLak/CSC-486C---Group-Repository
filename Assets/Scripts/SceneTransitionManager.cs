using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public GameObject traversableLayoutPrefab;
    private TraversableLayout traversableLayout;
    private PersistentData pd;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        // Subscribe to appropriate unity events
        EventSystem es = GameObject.FindWithTag("Event Sytem").GetComponent<EventSystem>();
        if (es) 
        {
            es.ExitLobbyToLayout.AddListener(SceneSwapToMapLayoutStart);
            es.ExitLayoutToEncounter.AddListener(SceneSwapToEncounter);
            es.ExitEncounterToLayout.AddListener(SceneSwapToMapLayoutProgress);
            es.ExitToMainMenu.AddListener(SceneSwapToMainMenu);
        }
        PersistentData pd = GameObject.FindWithTag("Persistent Data").GetComponent<PersistentData>();


    }


    /**********************************
    ************ Traversal ************
    **********************************/

    void CleanUpTraversal()
    {
        if (!traversableLayout) return;
        traversableLayout.DestroyEverything();
        Destroy(traversableLayout.gameObject);
        traversableLayout = null;
    }

    void SceneSwapToMapLayoutStart()
    {
        CleanUpTraversal();
        SceneManager.LoadScene("Scenes/Test");
        traversableLayout = Instantiate(traversableLayoutPrefab).GetComponent<TraversableLayout>();
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
        if (!traversableLayout) 
        {
            Debug.Log("Progress called out of order! No traversal manager initialized.");
            return;
        }
        traversableLayout.gameObject.SetActive(true);
        SceneManager.LoadScene("Scenes/LayoutTraversal");
        traversableLayout.DoProgress();
    }

    void SceneSwapToMainMenu()
    {

    }
}
