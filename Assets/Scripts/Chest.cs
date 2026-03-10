using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestItems chestItems;
    [SerializeField] private Mesh open;
    [SerializeField] private Mesh closed;
    private MeshFilter meshFilter;
    public bool isOpened { get; private set; }

    public bool isTrapRoom;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void Open()
    {
        if (isOpened) return;
        isOpened = true;
        meshFilter.sharedMesh = open;
        meshFilter.sharedMesh.RecalculateBounds();
        UIManager.instance.HideInteract();
        if (!isTrapRoom)
        {
            Acquirable randomCard = chestItems.GetCard();
            randomCard.Acquire();
            UIManager.instance.ShowChestUI(randomCard);
            Debug.Log("Get Card");
        }
        else
        {
            Acquirable goldItem = chestItems.GetGold();
            goldItem.Acquire();
            UIManager.instance.ShowChestUI(goldItem);
            Debug.Log("Get Gold");
        }

        Debug.Log("Chest open");

    }

}
