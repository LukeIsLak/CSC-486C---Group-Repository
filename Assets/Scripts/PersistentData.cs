using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    /* Traversal */
    public EncounterType    currentEncounterType;
    public bool             firstTimeAtLayout;
    public int              layoutSeed;

    /* Dungeon Encounter */
    public int              dungeonPoolSize,
                            dungeonIters,
                            dungeonItersPerSpecial,
                            dungeonSeed;
                            
    public static PersistentData instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        currentEncounterType    = EncounterType.None;
        firstTimeAtLayout       = true; 
    }
}
