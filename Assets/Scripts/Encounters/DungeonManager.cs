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
    public List<GameObject> specialPrefabs;
    public float roomScale;

    [Header("Data")]
    public DungeonData dungeonData;
    public DifficultyScaling diffScale;
    public RandomContext encRandomContext;
    
    [Header("Events")]
    public GameEvent EnterLayout;
    public GameEvent GenerationComplete;


    private LevelGenerator  lg;

    // Start is called before the first frame update
    void Start()
    {
        SetupDungeon();
    }

    void SetupDungeon()
    {
        if (!dungeonData.useSeed)
        dungeonData.dungeonSeed = encRandomContext.NextInt();
        lg = Instantiate(dungeonGeneratorPrefab, transform).GetComponent<LevelGenerator>();
        lg.Single               = Single;
        lg.DoubleI              = DoubleI;
        lg.DoubleL              = DoubleL;
        lg.Triple               = Triple;
        lg.Quad                 = Quad;
        lg.specialPrefabs       = specialPrefabs;
        lg.useSpecialRooms      = true;
        lg.roomScale            = roomScale;

        /* Poll info from persistent data */
        lg.recentPoolSize       = dungeonData.dungeonPoolSize;
        lg.desiredIterations    = dungeonData.dungeonIters;
        lg.iterationsPerSpecial = dungeonData.dungeonItersPerSpecial;
        lg.randomSeed           = dungeonData.dungeonSeed;
        lg.useSeed              = true;

        /* Generate */
        lg.DoGeneration();              
        lg.ClearGenerationObjects();
        DetermineTrapRooms();
        GenerationComplete.Raise();

    }

    void DetermineTrapRooms()
    {
        List<DungeonRoomScript> rooms = new List<DungeonRoomScript>(FindObjectsByType<DungeonRoomScript>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)); 

        int trapRoomsToCreate = (int)(diffScale.GetTrapRoomFraction() * rooms.Count);

        for (int i = 0; i < trapRoomsToCreate; i++)
        {
            DungeonRoomScript cur = rooms[encRandomContext.NextInt(0, rooms.Count)];
            cur.SetTrapRoom();
            rooms.Remove(cur);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //EnterLayout.Raise();
        }
    }
}
