using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FogObject : MonoBehaviour
{
    public float fadeTime;
    public float steps;
    private SpriteRenderer mat;
    void Awake()
    {
        mat = GetComponent<SpriteRenderer>();
    }
    public void DoFadeAndDelete()
    {
        if (mat == null)
        {
            Debug.Log("Huh?");
            return;
        }
        StartCoroutine(FadeAndDelete());
    }

    private IEnumerator FadeAndDelete()
    {
        float delay = fadeTime / steps;
        float dec   = 1f / steps;
        Color curColor = mat.material.color;

        while (curColor.a > 0)
        {
            curColor.a -= dec;
            mat.material.color = curColor;
            yield return new WaitForSeconds(delay);
        }

        Destroy(gameObject);
    }
}

