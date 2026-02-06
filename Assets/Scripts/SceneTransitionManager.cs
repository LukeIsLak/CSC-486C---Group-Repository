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
        SceneManager.LoadScene("Scenes/Enc");
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
        SceneManager.LoadScene("Scenes/Test");
        traversalManager.DoProgress();
    }


    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneSwapToMapLayoutStart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneSwapToEncounter();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneSwapToMapLayoutProgress();
        }
    }
}
