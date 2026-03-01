using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ChestItems chestItems;
    [SerializeField] private float validFacingAngle = 0.7f;
    private Camera cam;
    private bool isInRange;
    private bool isOpened;
    public bool canInteract { get; private set; }

    public bool isTrapRoom;

    // Update is called once per frame
    void Update()
    {
        if(isOpened || !isInRange || cam == null)
        {
            UIManager.instance.HideInteract();
            canInteract = false;
            return;
        }

        Vector3 camToChest = (transform.position - cam.transform.position).normalized;
        float dot = Vector3.Dot(cam.transform.forward,camToChest);
        canInteract = dot >= validFacingAngle;

        if (canInteract)
        {
            UIManager.instance.ShowInteract();
        }
        else
        {
            UIManager.instance.HideInteract();
        }
    }
    public void Open()
    {
        if (!canInteract || isOpened) return;
        isOpened = true;
        UIManager.instance.HideInteract();
        if (isTrapRoom)
        {
            chestItems.GetCard().Acquire();
        }
        else
        {
            chestItems.GetGold().Acquire();
        }

        Debug.Log("Chest open");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        isInRange = true;
        if(cam == null) cam = Camera.main;

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = false;
        UIManager.instance.HideInteract();
    }
}
