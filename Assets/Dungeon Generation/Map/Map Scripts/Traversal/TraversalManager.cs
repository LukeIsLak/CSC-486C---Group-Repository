using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TraversalManager : MonoBehaviour
{
    [Header("Required References")]
    public GameObject traversableLayoutPrefab;
    public Camera sceneCamera;
    public PlayerInventory inventory;

    [Header("Data")]
    public LayoutData layoutData;

    [Header("Events")]
    public GameEvent EnterEncounter;
    public GameEvent ExitToMenu;
    public GameEvent LayoutInitialized;

    private TraversableLayout traversableLayout;
    private bool respondToInputs = true;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        /* Since we store the parameters before first generation, we can regenerate
        and update to keep progress. */
        InitializeLayout();
        MapEncounter lastFinished = traversableLayout.DoProgress(layoutData.completedIndices);
        respondToInputs = true;

        sceneCamera.transform.position = lastFinished.transform.position + new Vector3(0f, 8, 0f);
        LayoutInitialized.Raise();
    }

    void InitializeLayout()
    {
        GameObject tmp = Instantiate(traversableLayoutPrefab);
        traversableLayout = tmp.GetComponent<TraversableLayout>();
        traversableLayout.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!respondToInputs) return;
        if (Input.GetKeyDown(KeyCode.Return) && traversableLayout.curSelectedEncounter)
        {
            // EnterSelectedEncounter();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // ExitToMenu.Raise();
        }

        if (Input.GetMouseButton(0))
        {
            Vector2  diff   = Mouse.current.delta.ReadValue();
            float offsetH   = -diff[0]/50f;
            float offsetV   = -diff[1]/50f;

            Vector3 curPos = sceneCamera.transform.position;
            curPos      += new Vector3(offsetH, 0f, offsetV);
            curPos[0]   = Mathf.Min(layoutData.maxWidth * layoutData.encounterSep / 2, curPos[0]);
            curPos[0]   = Mathf.Max(-layoutData.maxWidth * layoutData.encounterSep / 2, curPos[0]);
            curPos[2]   = Mathf.Min(0, curPos[2]);
            curPos[2]   = Mathf.Max(-layoutData.depth * layoutData.layerDistance, curPos[2]);

            sceneCamera.transform.position = curPos;
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

    public void EnterSelectedEncounter()
    {
        layoutData.currentEncounter = traversableLayout.curSelectedEncounter.encounter;
        layoutData.completedIndices.Add(traversableLayout.curSelectedEncounter.index);
        respondToInputs = false;
        EnterEncounter.Raise();
    }

    public void ClearInventoryBuffer()
    {
        inventory.ClearBuffer();
    }

    public void ToggleInputs()
    {
        respondToInputs = !respondToInputs;
        traversableLayout.respondToInputs = respondToInputs;
    }
}
