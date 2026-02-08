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
        /* Since we store the parameters before first generation, we can regenerate
        and update to keep progress. */
        InitializeLayout();
        traversableLayout.DoProgress(layoutData.completedIndices);
        respondToInputs = true;
    }

    void InitializeLayout()
    {
        layoutData.shouldGenerate       = false;
        GameObject tmp = Instantiate(traversableLayoutPrefab);
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
            layoutData.completedIndices.Add(traversableLayout.curSelectedEncounter.index);
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
