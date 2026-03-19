using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTriggerStarter : MonoBehaviour
{
    public string playerTag;
    public bool started = false;

    public List<GameObject> sequencesObjects;

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag(playerTag)) return;
        StartTrap();
    }

    void StartTrap()
    {
        if (started) return;
        started = true;
        foreach (GameObject so in sequencesObjects)
        {
            ITrapSequence s = so.GetComponent<ITrapSequence>();
            s?.Begin();
        }
        Destroy(gameObject);
    }
}
