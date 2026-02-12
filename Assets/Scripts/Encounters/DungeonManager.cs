using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("GrowthPLG Prefabs")]
    public GameObject   dungeonGeneratorPrefab;
    public GameObject   Single;                     // 1 connection
    public GameObject   DoubleI;                    // 2 connections in a straight line
    public GameObject   DoubleL;                    // 2 connections at a right angle
    public GameObject   Triple;                     // 3 connections
    public GameObject   Quad;                       // 4 connections

    [Header("Data")]
    public DungeonData dungeonData;
    
    [Header("Events")]
    public GameEvent EnterLayout;


    private LevelGenerator  lg;

    // Start is called before the first frame update
    void Start()
    {
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
        lg.roomScale            = 6f*2.5f;

        /* Poll info from persistent data */
        lg.recentPoolSize       = dungeonData.dungeonPoolSize;
        lg.desiredIterations    = dungeonData.dungeonIters;
        lg.iterationsPerSpecial = dungeonData.dungeonItersPerSpecial;
        lg.randomSeed           = dungeonData.dungeonSeed;
        lg.useSeed              = dungeonData.useSeed;

        /* Generate */
        lg.DoGeneration();              
        lg.ClearGenerationObjects();    
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
