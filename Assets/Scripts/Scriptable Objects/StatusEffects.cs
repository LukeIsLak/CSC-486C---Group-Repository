using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType {
    DamageOverTime,
    Freeze,
    Knockback,
    Stop,
    None
}

public class StatusEffects : ScriptableObject {
    public string name;
    public StatusEffectType type;
}
