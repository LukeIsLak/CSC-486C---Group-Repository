using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlayUI : MonoBehaviour
{
    [SerializeField] Image overlayImage;
    [SerializeField] float flashAlpha = 0.5f;
    [SerializeField] float holdTime = 0.05f;
    [SerializeField] float fadeSpeed = 4f;
    private Coroutine flashRoutine;
    private Health health;
    private void Awake()
    {
        if (overlayImage != null)
        {
            var color = overlayImage.color;
            color.a = 0;
            overlayImage.color = color;
        }
    }


    public void BindHealthUI(Health health)
    {
        if (this.health != null)
        {
            this.health.OnDamaged -= ShowDamageOverlay;
        }
        this.health = health;

        if (this.health != null)
        {
            this.health.OnDamaged += ShowDamageOverlay;
        }
    }

    private void ShowDamageOverlay()
    {
        if(flashRoutine != null) StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (overlayImage == null)
            yield break;

        Color c = overlayImage.color;
        c.a = flashAlpha;
        overlayImage.color = c;

        yield return new WaitForSeconds(holdTime);

        while(overlayImage.color.a > 0.01f)
        {
            c = overlayImage.color;
            c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
            overlayImage.color = c;
            yield return null;
        }
        c = overlayImage.color;
        c.a = 0f;
        overlayImage.color = c;
        flashRoutine = null;
    }
}
