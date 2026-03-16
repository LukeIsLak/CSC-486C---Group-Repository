using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSequence : MonoBehaviour, ITrapSequence
{
    // Start is called before the first frame update
    public bool started = false;
    public GameObject chestObject;

    // Update is called once per frame
    void Awake()
    {
        started = false;
    }

    public void Begin()
    {
        started = true;
    }

    public void OnClear()
    {
        if (!chestObject || !started) return;
        chestObject.SetActive(true);
    }
}
