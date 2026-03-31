using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/StatusEffect/Freeze")]
public class Freeze : StatusEffects {
    public float freezeDuration;
    public bool hasOngoingPart = false;

    /*Particle Effects*/
    public GameObject ongoingPart;
}
