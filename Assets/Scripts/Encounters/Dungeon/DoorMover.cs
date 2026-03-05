using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMover : MonoBehaviour
{

    public float moveDistance;
    public float moveTime;

    public void Lower()
    {
        Vector3 curPos = transform.position;
        StartCoroutine(MoveToPosition(curPos, curPos + Vector3.down * moveDistance, moveTime));
    }
    public void Raise()
    {
        Vector3 curPos = transform.position;
        StartCoroutine(MoveToPosition(curPos, curPos + Vector3.up * moveDistance, moveTime));
        
    }
    public IEnumerator MoveToPosition(Vector3 start, Vector3 end, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            float t = elapsed / time;
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        transform.position = end;
    }
}
