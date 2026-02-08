using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Events")]
    public GameEvent EnterLayout;
    public GameEvent ExitToMenu;

    void Start()
    {
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
