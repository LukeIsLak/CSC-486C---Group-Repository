using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentReferences : MonoBehaviour
{
    public List<ScriptableObject> persistingContainers;

    public static PersistentReferences instance;
    void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
