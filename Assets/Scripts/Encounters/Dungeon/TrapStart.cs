using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapStart : MonoBehaviour
{
    public string playerTag;
    public bool started = false;

    public List<TrapSequence> sequences;

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag(playerTag)) return;
        StartTrap();
    }

    void StartTrap()
    {
        if (started) return;
        started = true;
        foreach (TrapSequence seq in sequences) seq.Begin();
    }

}
