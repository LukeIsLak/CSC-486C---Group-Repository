using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationEvent : MonoBehaviour
{   
    private PlayerCharacter character;
    private void Awake()
    {
        character = GetComponentInParent<PlayerCharacter>();
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
