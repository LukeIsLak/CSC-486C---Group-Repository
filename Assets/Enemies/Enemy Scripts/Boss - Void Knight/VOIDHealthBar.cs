using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class VOIDHealthBar : MonoBehaviour
{

    public Image imageToScale;
    private float maxValue;
    private float value;

    private RectTransform rt;
    private float maxWidth;

    void Awake()
    {
        rt = imageToScale.GetComponent<RectTransform>();
    }
    public void SetMax(float health)
    {
        maxValue = health;
    }

    public void SetHealth(float health)
    {
        value = health;

        Vector3 localScale = rt.localScale;
        localScale.x = value / maxValue;
        rt.localScale = localScale;
    }
}
