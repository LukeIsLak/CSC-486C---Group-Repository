using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentReferences : MonoBehaviour
{
    public List<ScriptableObject> persistingContainers;

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
