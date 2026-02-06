using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    /* Store data to be referenced between scenes.
    Where applicable, managers for individual scenes should
    reference this script where 
    */

    /* Traversal */
    public EncounterType    currentEncounterType;
    public bool             firstTimeAtLayout;
    public int              layoutSeed, roomSeed;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        currentEncounterType    = EncounterType.None;
        firstTimeAtLayout       = true; 
    }
}
