using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private PlayerCharacter character;

    public void BindPlayerUI(PlayerCharacter character)
    {
        this.character = character;
    }

    private void Update()
    {
        if (character == null) return;
        slider.value = character.nextDashRemaining;
    }
}
