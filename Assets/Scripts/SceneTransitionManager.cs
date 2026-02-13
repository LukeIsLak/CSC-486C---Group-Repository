using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scenes")]
    public SceneField   lobbyScene;
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

    private void ForceMouseOn()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SceneSwapToLobby()
    {
        ForceMouseOn();
        SceneManager.LoadScene(lobbyScene);
    }
    
    public void SceneSwapToMapLayout()
    {   
        ForceMouseOn();
        SceneManager.LoadScene(layoutScene);
    }

    public void SceneSwapToEncounter()
    {
        EncounterInfo encType = layoutData.currentEncounter;

        if (encType.scene == null)
        Debug.Log("Nothing to do for you with this encounter type.");
        ForceMouseOn();
        SceneManager.LoadScene(encType.scene);
    }

    public void SceneSwapToMainMenu()
    {
        ForceMouseOn();
        SceneManager.LoadScene(menuScene);
    }
}
