using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoorSequence : TrapSequence
{
    public bool started = false;
    public DoorMover trapDoors;
    public override void Begin()
    {
        started = true;
        trapDoors.Lower();
        StartCoroutine(RaiseAfterDelay());
    }

    public void OnClear()
    {
        if (!started) return;
        trapDoors.Raise();
    }

    public IEnumerator RaiseAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        OnClear();
    }
}
