using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("Scenes")]
    public SceneField   lobbyScene;
    public SceneField   layoutScene;
    public SceneField   menuScene;
    public SceneField   deathScene; 

    [Header("Data")]
    public LayoutData   layoutData;
    public GameEvent SceneChanging;

    public float fadeTime = 1;
    public float steps = 30f;
    public RawImage image;

    /* Added this back... Assuming it will be ever-present */
    public static SceneTransitionManager instance;
    public bool isTransitioning = false;
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
        Color curColor = image.material.color;
        curColor.a = 1f;
        image.material.color = curColor;
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
        DoLoadWithFade(lobbyScene, true, true);
        // SceneManager.LoadScene(lobbyScene);
    }
    
    public void SceneSwapToMapLayout()
    {   
        DoLoadWithFade(layoutScene, true, false);
    }

    public void SceneSwapToEncounter()
    {
        EncounterInfo encType = layoutData.currentEncounter;

        if (encType.scene == null)
        Debug.Log("Nothing to do for you with this encounter type.");
        DoLoadWithFade(encType.scene, true, true);
    }

    public void SceneSwapToMainMenu()
    {
        SceneManager.LoadScene(menuScene);

    }

    public void SceneSwapToMainMenuFade()
    {
        DoLoadWithFade(menuScene, true, true);
    }

    public void SceneSwapToDeath()
    {
        DoLoadWithFade(deathScene, true, true);
    }
    

    /**********************************
    ************ Fade Logic ************
    **********************************/
    private void DoLoadWithFade(SceneField scene, bool fadeIn, bool fadeOut)
    {
        StartCoroutine(LoadWithFade(scene, fadeIn, fadeOut)); 
    }  

    private IEnumerator LoadWithFade(SceneField scene, bool fadeIn, bool fadeOut)
    {
        if (fadeIn)
        {
            StartCoroutine(FadeIn());
            yield return new WaitForSeconds(fadeTime + 0.1f);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Color curColor = image.color;
            curColor.a = 1f;
            image.color = curColor;
            SceneManager.LoadScene(scene);
        }
        // Check if should fade out, then do it if so.
        if (fadeOut) 
        { 
            StartCoroutine(FadeOut());
        } 
        else
        {
            Color curColor = image.color;
            curColor.a = 0f;
            image.color = curColor;
        }
    }
    private IEnumerator FadeIn()
    {
        float delay = fadeTime / 30;
        float dec   = 1f / 30;
        // Force to transparent
        Color curColor = image.color;
        curColor.a = 0f;

        while (curColor.a < 1)
        {
            curColor.a += dec;
            image.color = curColor;
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator FadeOut()
    {
        float delay = fadeTime / steps;
        float dec   = 1f / steps;
        Color curColor = image.color;
        curColor.a = 1f;

        while (curColor.a > 0)
        {
            curColor.a -= dec;
            image.color = curColor;
            yield return new WaitForSeconds(delay);
        }
    }
}


