using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private PersistentData  pd;

    [Header("Raisable Events")]
    public GameEvent EnterLayout;
    public GameEvent ExitToMenu;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            ExitToMenu.Raise();
        }

    }
}
