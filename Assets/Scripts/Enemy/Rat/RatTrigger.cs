using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerZone { Inner, Outer }

public class RatTrigger : MonoBehaviour
{
    public TriggerZone zone;
    public Rat owner;

    void OnTriggerEnter(UnityEngine.Collider other) {
        if (owner != null) owner.OnTriggerZone(other, zone, true);
    }

    void OnTriggerExit(UnityEngine.Collider other) {
        if (owner != null) owner.OnTriggerZone(other, zone, false);
    }
}