using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimationEvent : MonoBehaviour
{   
    private PlayerCharacter character;

    public void BindPlayer(PlayerCharacter character)
    {
        this.character = character;
    }

    public void AttackRayCast()
    {
        if (character == null) 
        { 
            Debug.Log("character null"); 
            return; 
        }

        character.AttackRaycast();
    }

    public void EndAttack()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.EndAttack();
    }

    public void OpenComboWindow()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.OpenComboWindow();
    }

    public void CloseComboWindow()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.CloseComboWindow();
    }

    public void EnableSwordHitbox()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.EnableSwordColider();
    }

    public void DisableSwordHitbox()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.DisableSwordColider();
    }
}
