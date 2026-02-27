using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoors : TrapSequence
{
    public GameObject trapDoorPrefab;
    public float trapDoorPosition;
    public bool started = false;
    public override void Begin()
    {
        started = true;
        Vector3 curPos;
        Vector3 curAng;
        GameObject curDoor;
        
        // Positive X
        curPos  = Vector3.right * trapDoorPosition;
        curAng  = new Vector3(0f, 0f, 0f);
        curDoor = Instantiate(trapDoorPrefab, transform);
        curDoor.transform.localPosition = curPos;
        curDoor.transform.eulerAngles   = curAng;

        // Negative X
        curPos  = Vector3.left * trapDoorPosition;
        curAng  = new Vector3(0f, 180f, 0f);
        curDoor = Instantiate(trapDoorPrefab, transform);
        curDoor.transform.localPosition = curPos;
        curDoor.transform.eulerAngles   = curAng;

        // Positive Z
        curPos  = Vector3.forward * trapDoorPosition;
        curAng  = new Vector3(0f, 90f, 0f);
        curDoor = Instantiate(trapDoorPrefab, transform);
        curDoor.transform.localPosition = curPos;
        curDoor.transform.eulerAngles   = curAng;

        // Negative Z
        curPos  = Vector3.back * trapDoorPosition;
        curAng  = new Vector3(0f, -90f, 0f);
        curDoor = Instantiate(trapDoorPrefab, transform);
        curDoor.transform.localPosition = curPos;
        curDoor.transform.eulerAngles   = curAng;
    }

    public void OnClear()
    {
        if (!started) return;
        Destroy(gameObject);
    }
}
