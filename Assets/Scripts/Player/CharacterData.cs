using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public float maxHealth = 100f;
    public float currentHealth = 0f;
    public float moveSpeed = 1.0f;

    public float baseHealth = 100f;
    public float baseSpeed = 1.0f;

}
