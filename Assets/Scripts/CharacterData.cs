using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 1.0f;
}
