using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scenes")]
    public SceneField   merchantScene;
    public SceneField   treasureScene;
    public SceneField   dungeonScene;
    public SceneField   bossScene;
    public SceneField   layoutScene;
    public SceneField   menuScene;

    [Header("Data")]
    public LayoutData   layoutData;

    /* Added this back... Assuming it will be ever-present */
    public static SceneTransitionManager instance;
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

    /**********************************
    ************ Traversal ************
    **********************************/

    public void SceneSwapToMapLayout()
    {
        SceneManager.LoadScene(layoutScene);
    }

    public void SceneSwapToEncounter()
    {
        EncounterType encType = layoutData.currentEncounter;
    
        if (encType == EncounterType.None) 
        {
            Debug.Log("No encounter type provided.");
            return;
        }
        if (encType == EncounterType.Merchant)
        {
            SceneManager.LoadScene(layoutScene);
            return;
        }
        if (encType == EncounterType.Treasure)
        {
            SceneManager.LoadScene(treasureScene);
            return;

        }
        if (encType == EncounterType.Dungeon)
        {
            SceneManager.LoadScene(dungeonScene);
            return;

        }
        if (encType == EncounterType.Boss)
        {
            SceneManager.LoadScene(bossScene);  
            return;

        }
        Debug.Log("Nothing to do for you with this encounter type.");
    }

    public void SceneSwapToMainMenu()
    {
        SceneManager.LoadScene(menuScene);
    }
}
