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

    public void AttackRayCast()
    {
        character.AttackRaycast();
    }

    public void EndAttack()
    {
        character.EndAttack();
    }

    public void OpenComboWindow()
    {
        character.OpenComboWindow();
    }

    public void CloseComboWindow()
    {
        character.CloseComboWindow();
    }
}
