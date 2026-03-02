using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestItems chestItems;
    public bool isOpened { get; private set; }

    public bool isTrapRoom;

    public void Open()
    {
        if (isOpened) return;
        isOpened = true;
        UIManager.instance.HideInteract();
        if (!isTrapRoom)
        {
            //chestItems.GetCard().Acquire();
            Debug.Log("Get Card");
        }
        else
        {
            //chestItems.GetGold().Acquire();
            Debug.Log("Get Gold");
        }

        Debug.Log("Chest open");

    }

}
