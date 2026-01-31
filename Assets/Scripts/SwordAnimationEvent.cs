using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationEvent : MonoBehaviour
{   
    private Character character;
    private void Awake()
    {
        character = GetComponentInParent<Character>();
    }

    public void RayCast()
    {
        character.Raycast();
    }

    public void ResetAttack()
    {
        character.ResetAttack();
    }
}
