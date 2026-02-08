using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TraversalManager : MonoBehaviour
{
    [Header("Required Prefabs")]
    public GameObject traversableLayoutPrefab;
    
    [Header("Data")]
    public LayoutData layoutData;

    [Header("Events")]
    public GameEvent EnterEncounter;

    private TraversableLayout traversableLayout;
    private bool respondToInputs = true;
    

    void Start()
    {
        /* Really, we should be storing the layout and reconstructing it as needed.
        But that can come later. For now, we assume that if it's not existent, we are visiting 
        the layout for the first time. If it already exists, we're returning after an encounter.*/
        
        traversableLayout = GameObject.FindWithTag("Traversable Layout")?.GetComponent<TraversableLayout>();

        if (!traversableLayout) 
        { 
            InitializeLayout();
        }

        traversableLayout.SetActive(true);
        if (shouldDoProgress)
        {
            traversableLayout.DoProgress();
        }

        respondToInputs = true;
    }

    void InitializeLayout()
    {
        layoutData.shouldGenerate       = false;
        layoutData.shouldDoProgress     = true;
        GameObject tmp = Instantiate(traversableLayoutPrefab);
        DontDestroyOnLoad(tmp);
        traversableLayout = tmp.GetComponent<TraversableLayout>();
        traversableLayout.depth         = layoutData.depth;
        traversableLayout.maxWidth      = layoutData.depth;
        traversableLayout.randomSeed    = layoutData.randomSeed;
        traversableLayout.useSeed       = layoutData.useSeed;
        traversableLayout.Initialize();
    }
    // Update is called once per frame
    void Update()
    {
        if (respondToInputs && Input.GetKeyDown(KeyCode.Return) && traversableLayout.curSelectedEncounter)
        {
            layoutData.currentEncounter = traversableLayout.curSelectedEncounter.encounter;
            traversableLayout.gameObject.SetActive(false);
            respondToInputs = false;
            EnterEncounter.Raise();
        }
    }

    void CleanUpTraversal()
    {
        if (!traversableLayout) return;
        traversableLayout.DestroyEverything();
        Destroy(traversableLayout.gameObject);
    }

    public void DestroySelf()
    {
        // Listeners
        CleanUpTraversal();
        Destroy(gameObject);
    }
}
