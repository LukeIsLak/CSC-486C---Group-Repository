using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SpecTraversalManager : MonoBehaviour
{
    [Header("Required References")]
    public GameObject traversableLayoutPrefab;

    [Header("Data")]
    public LayoutData layoutData;

    [Header("Events")]
    public GameEvent EnterEncounter;
    private TraversableLayout traversableLayout;
    
    void Start()
    {
        InitializeLayout();
        MapEncounter lastFinished = traversableLayout.DoProgress(layoutData.completedIndices);
        // Assume selected encounter is set properly.
        EnterSelectedEncounter();
    }

    void InitializeLayout()
    {
        GameObject tmp = Instantiate(traversableLayoutPrefab);
        traversableLayout = tmp.GetComponent<TraversableLayout>();
        traversableLayout.Initialize();
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

    public void EnterSelectedEncounter()
    {
        EnterEncounter.Raise();
    }
}
