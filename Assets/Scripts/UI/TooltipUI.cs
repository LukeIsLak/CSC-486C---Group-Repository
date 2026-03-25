using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI instance;

    public TextMeshProUGUI headerText;
    public TextMeshProUGUI contentText;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private int characterWrapLimit;
    [SerializeField] private RectTransform rectTransform;
    private void Awake()
    {
        instance = this;
        Hide();

    }
    public void Show(string header, string content)
    {
        gameObject.SetActive(true);
        headerText.text = header;
        contentText.text = content;
        int headerLength = headerText.text.Length;
        int contentLength = contentText.text.Length;
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 normalizedPos = new Vector2(mousePos.x/Screen.width, mousePos.y/Screen.height);
        rectTransform.pivot = CalculatePivot(normalizedPos);
        rectTransform.position = mousePos;

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private Vector2 CalculatePivot(Vector2 position)
    {
        var pivotTopLeft = new Vector2(-0.05f, 1.05f);
        var pivotTopRight = new Vector2(1.05f, 1.05f);
        var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
        var pivotBottomRight = new Vector2(1.05f, -0.05f);

        if (position.x < 0.5f && position.y >= 0.5f)
        {
            return pivotTopLeft;
        }
        else if (position.x > 0.5f && position.y >= 0.5f)
        {
            return pivotTopRight;
        }
        else if (position.x <= 0.5f && position.y < 0.5f)
        {
            return pivotBottomLeft;
        }
        else
        {
            return pivotBottomRight;
        }
    }
}
