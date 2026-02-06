using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TraversalManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject traversableLayoutPrefab;
    private PersistentData pd;
    private EventSystem es;

    private TraversableLayout traversableLayout;

    private bool respondToInputs = true;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        pd = GameObject.FindWithTag("Persistent Data")?.GetComponent<PersistentData>();
        es = GameObject.FindWithTag("Event System")?.GetComponent<EventSystem>();
        es?.ExitToMainMenu.AddListener(CleanUpTraversal);

        GameObject tmp = Instantiate(traversableLayoutPrefab, transform);
        traversableLayout = tmp.GetComponent<TraversableLayout>();
        traversableLayout.depth = 10;
        traversableLayout.maxWidth = 7;
        traversableLayout.randomSeed = (int)System.DateTime.Now.Ticks;
        traversableLayout.useSeed = true;
        traversableLayout.Initialize();
        // Handle camera placement, layout placement etc.
    }

    // What to do when transitioning in/out of the scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LayoutTraversal")
        {
            traversableLayout.gameObject.SetActive(true);
            respondToInputs = true;
        }
        else
        {
            traversableLayout.gameObject.SetActive(false);
            respondToInputs = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (respondToInputs && Input.GetKeyDown(KeyCode.Return) && traversableLayout.curSelectedEncounter)
        {
            pd.currentEncounterType = traversableLayout.curSelectedEncounter.encounter;
            es.ExitLayoutToEncounter.Invoke();
        }
    }

    void CleanUpTraversal()
    {
        if (!traversableLayout) return;
        traversableLayout.DestroyEverything();
        Destroy(traversableLayout.gameObject);
        Destroy(gameObject);
    }
}
