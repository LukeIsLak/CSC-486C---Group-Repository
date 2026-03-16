using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ChestItems chestItems;
    public List<GameObject> sequenceObjects;

    [Header("Opening Animation")]
    [SerializeField] private Mesh open;
    [SerializeField] private Mesh closed;
    [SerializeField] private Transform cardSpawnPosition;
    [SerializeField] private CardRewardDisplay cardPrefab;
    private MeshFilter meshFilter;
    public bool isOpened { get; private set; }

    public bool isGoldChest;

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
        if (!isGoldChest)
        {
            CardAcquirable randomCard = (CardAcquirable)chestItems.GetCard();
            randomCard.Acquire();
            CardRewardDisplay card = Instantiate(cardPrefab, cardSpawnPosition);
            card.Init(randomCard.card);
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

        // Begin all trap sequences 
        foreach (GameObject so in sequenceObjects)
        {
            ITrapSequence s = so.GetComponent<ITrapSequence>();
            s?.Begin();
        }
    }

    private IEnumerator PopUpUIRoutine(Acquirable randomCard, CardRewardDisplay card)
    {
        yield return new WaitForSeconds(2f);
        UIManager.instance.ShowChestUI(randomCard);
        Destroy(card.gameObject);

    }

}
