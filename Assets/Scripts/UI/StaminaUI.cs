using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;
    [SerializeField] private Color readyColor;
    [SerializeField] private Color recoveringColor;
    private PlayerCharacter character;

    public void BindPlayerUI(PlayerCharacter character)
    {

        if (this.character != null)
        {

            this.character.OnDashNotify -= HandleStaminaBarColor;
        }
        this.character = character;

        if (this.character != null)
        {
            Debug.Log("Character bound to stamina UI");
            this.character.OnDashNotify += HandleStaminaBarColor;
            HandleStaminaBarColor(this.character.nextDashRemaining >= 1f);
        }
    }
    private void OnDestroy()
    {
        if (character != null)
        {
            character.OnDashNotify -= HandleStaminaBarColor;
        }
    }

    private void Update()
    {
        if (character == null) return;
        staminaFill.fillAmount = character.nextDashRemaining *0.5f;

    }

    private void HandleStaminaBarColor(bool isReady)
    {
        Debug.Log("HandleStaminaBarColor called: " + isReady);
        staminaFill.color = isReady ? readyColor : recoveringColor;
    }
}
