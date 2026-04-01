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

    public void EnableSwordHitboxA()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.EnableSwordColiderA();
    }

    public void DisableSwordHitboxA()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.DisableSwordColiderA();
    }

    public void EnableSwordHitboxB()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.EnableSwordColiderB();
    }

    public void DisableSwordHitboxB()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.DisableSwordColiderB();
    }

    public void EnableSwordHitboxC()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.EnableSwordColiderC();
    }

    public void DisableSwordHitboxC()
    {
        if (character == null)
        {
            Debug.Log("character null");
            return;
        }
        character.DisableSwordColiderC();
    }
}
