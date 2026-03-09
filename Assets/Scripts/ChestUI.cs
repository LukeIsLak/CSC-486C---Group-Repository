using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeSpeed = 5f;
    [SerializeField] float displayDuration = 5f;
    private Coroutine fadeCoroutine;

    public void ShowUI(Acquirable cardData)
    {
        itemImage.sprite = cardData.icon;
        itemName.text = cardData.itemName;
        itemImage.preserveAspect = true;
        gameObject.SetActive(true);
        if(fadeCoroutine != null ) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        float t = 0f;
        canvasGroup.alpha = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayDuration);
        t= 0f;

        while(t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        gameObject.SetActive(false);
        fadeCoroutine = null;
    }

}
