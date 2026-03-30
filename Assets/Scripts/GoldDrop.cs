using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GoldDrop : MonoBehaviour
{
    public PlayerInventory playerInventory;
    private int amount = 1;
    public TextMeshPro textMeshPro;
    public GameObject graphicsContainer;

    [Header("Animation Parameters")]
    public float floatHeight = 1f;
    public float floatTime = 2f; 
    private float curTime = 0f;

    public void DoGoldAcquire(int amount)
    {
        this.amount = amount;
        playerInventory.addCurrency(amount);
        textMeshPro.text = "+" + amount.ToString();
        graphicsContainer.SetActive(true);
        StartCoroutine(FloatUpThenDelete());
    }

    public IEnumerator FloatUpThenDelete()
    {
        Vector3 desiredPos = transform.position + floatHeight * Vector3.up;
        Vector3 startPos   = transform.position;
        while (curTime < floatTime)
        {
            curTime += Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(startPos, desiredPos, curTime / floatTime);
            yield return new WaitForFixedUpdate();
        }
        Destroy(this.gameObject);
    }
}
