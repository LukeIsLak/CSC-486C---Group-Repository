using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public GameObject traversalManagerPrefab;
    private TraversalManager traversalManager;

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

    }


    /**********************************
    ************ Traversal ************
    **********************************/

    void CleanUpTraversal()
    {
        if (!traversalManager) return;
        traversalManager.DestroyEverything();
        Destroy(traversalManager.gameObject);
        traversalManager = null;
    }

    void SceneSwapToMapLayoutStart()
    {
        CleanUpTraversal();
        SceneManager.LoadScene("Scenes/Test");
        traversalManager = Instantiate(traversalManagerPrefab).GetComponent<TraversalManager>();
    }

    void SceneSwapToEncounter()
    {
        if (!traversalManager) 
        {
            Debug.Log("Progress called out of order! No traversal manager initialized.");
            return;
        }
        if (!traversalManager.curSelectedEncounter)
        {
            Debug.Log("No encounter selected!");
            return;
        }

        // To do: determine encounter type
        SceneManager.LoadScene("Scenes/Encounters/Merchant");
        traversalManager.gameObject.SetActive(false);
    }

    void SceneSwapToMapLayoutProgress()
    {
        if (!traversalManager) 
        {
            Debug.Log("Progress called out of order! No traversal manager initialized.");
            return;
        }
        traversalManager.gameObject.SetActive(true);
        SceneManager.LoadScene("Scenes/LayoutTraversal");
        traversalManager.DoProgress();
    }

    void SceneSwapToMainMenu()
    {

    }
}
