using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    private PersistentData  pd;
    private EventSystem     es;

    void Start()
    {
        pd = PersistentData.instance;
        es = EventSystem.instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            es?.ExitEncounterToLayout.Invoke();
        }
    }
}
