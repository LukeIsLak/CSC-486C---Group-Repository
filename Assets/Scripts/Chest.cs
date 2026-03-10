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
    [SerializeField] private Transform cardSpawnPosition;
    [SerializeField] private CardRewardDisplay cardPrefab;
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
            CardRewardDisplay card = Instantiate(cardPrefab, cardSpawnPosition);
            card.Init(randomCard.itemDescription, randomCard.itemName, "10");
            StartCoroutine(PopUpUIRoutine(randomCard, card));
            Debug.Log("Get Card");
        }
        else
        {
            Acquirable goldItem = chestItems.GetGold();
            goldItem.Acquire();
            UIManager.instance.ShowChestUI(goldItem);
            Debug.Log("Get Gold");
        }
    }

    private IEnumerator PopUpUIRoutine(Acquirable randomCard, CardRewardDisplay card)
    {
        yield return new WaitForSeconds(2f);
        UIManager.instance.ShowChestUI(randomCard);
        Destroy(card.gameObject);

    }

}
