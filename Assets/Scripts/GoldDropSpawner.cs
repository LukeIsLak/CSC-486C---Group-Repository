using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GoldDropSpawner : MonoBehaviour
{
    public GameObject goldDropPrefab;
    public int amount = 1;
    void OnDestroy()
    {
        GameObject goldDrop = Instantiate(goldDropPrefab);
        goldDrop.transform.position = transform.position;
        goldDrop.GetComponent<GoldDrop>().DoGoldAcquire(amount);
    }
}
