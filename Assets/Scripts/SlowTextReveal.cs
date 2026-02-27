using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SlowTextReveal : MonoBehaviour
{
    // Start is called before the first frame update
    [TextArea] public string message;
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(TextRoutine());
    }

    private IEnumerator TextRoutine()
    {
        yield return new WaitForSeconds(1f);
        string curText = "";

        foreach (char c in message)
        {
            if (c == '\n') yield return new WaitForSecondsRealtime(1.5f);
            curText += c;
            text.SetText(curText);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}
