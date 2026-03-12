using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltObject : MonoBehaviour
{
    public LineRenderer line;

    [SerializeField] private float ttl = 5f;

    void Start()
    {
        StartCoroutine(timeToLive(ttl));
    }

    private IEnumerator timeToLive(float dur) {
        yield return new WaitForSeconds(dur);
        Destroy(this.gameObject);
    }

}
