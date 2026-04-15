using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOnTrigger : MonoBehaviour
{
    public GameEvent RaiseOnTriggerEnter;
    public string targetTag;
    public bool destroyOnRaise = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        RaiseOnTriggerEnter.Raise();
        if (destroyOnRaise) Destroy(gameObject);
    }
}
