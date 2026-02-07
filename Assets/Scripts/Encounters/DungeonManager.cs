using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private PersistentData  pd;
    private EventSystem     es;

    [Header("GrowthPLG Prefabs")]
    public GameObject   dungeonGeneratorPrefab;
    public GameObject   Single;                     // 1 connection
    public GameObject   DoubleI;                    // 2 connections in a straight line
    public GameObject   DoubleL;                    // 2 connections at a right angle
    public GameObject   Triple;                     // 3 connections
    public GameObject   Quad;                       // 4 connections
    private LevelGenerator  lg;

    // Start is called before the first frame update
    void Start()
    {
        pd = PersistentData.instance;
        es = EventSystem.instance;
        SetupDungeon();
    }

    void SetupDungeon()
    {
        lg = Instantiate(dungeonGeneratorPrefab, transform).GetComponent<LevelGenerator>();
        lg.Single               = Single;
        lg.DoubleI              = DoubleI;
        lg.DoubleL              = DoubleL;
        lg.Triple               = Triple;
        lg.Quad                 = Quad;
        lg.roomScale            = 6;

        /* Poll info from persistent data */
        if (!pd) { lg.DoGeneration(); return; }
        lg.recentPoolSize       = pd.dungeonPoolSize;
        lg.desiredIterations    = pd.dungeonIters;
        lg.iterationsPerSpecial = pd.dungeonItersPerSpecial;
        lg.randomSeed           = pd.dungeonSeed;
        lg.useSeed              = true;

        /* Generate */
        lg.DoGeneration();              
        lg.ClearGenerationObjects();    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            es?.ExitEncounterToLayout.Invoke();
        }
    }
}
