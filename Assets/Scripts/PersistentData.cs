using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData instance;


    /* Traversal */
    public EncounterType    currentEncounterType;
    public bool             firstTimeAtLayout;
    public int              layoutSeed, roomSeed;

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
