using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    private PersistentData  pd;
    [Header("Raisable Events")]
    public GameEvent EnterLayout;

    void Start()
    {
        pd = PersistentData.instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterLayout.Raise();
        }
    }
}
