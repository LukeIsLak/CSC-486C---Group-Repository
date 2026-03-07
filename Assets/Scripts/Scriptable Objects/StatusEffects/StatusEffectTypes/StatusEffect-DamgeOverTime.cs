using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Data/StatusEffect/DamageOverTime")]
public class DamageOverTime : StatusEffects {
    public float ticksPerSecond;
    public float tickDamage;
    public int totalTicks;

    /*I.e do we extend the duration or do we stack them*/
    public bool isStackable;
}
