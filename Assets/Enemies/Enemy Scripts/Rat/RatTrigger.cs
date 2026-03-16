using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerZone { Inner, Outer }
public enum TriggerType { Enter, Stay, Exit }

public class RatTrigger : MonoBehaviour
{
    public TriggerZone zone;
    public Rat owner;

    void OnTriggerEnter(Collider other) {
        if (owner != null) owner.OnTriggerZone(other, zone, TriggerType.Enter);
    }

    void OnTriggerStay(Collider other) {
        if (owner != null) owner.OnTriggerZone(other, zone, TriggerType.Stay);
    }

    void OnTriggerExit(Collider other) {
        if (owner != null) owner.OnTriggerZone(other, zone, TriggerType.Exit);
    }
}