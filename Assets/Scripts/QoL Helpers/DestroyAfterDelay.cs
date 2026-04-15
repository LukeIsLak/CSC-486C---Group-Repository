using System.Collections;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float delay = 1f;
    void Start()
    {
        StartCoroutine(DoDestroyAfterDelay());
    }

    private IEnumerator DoDestroyAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
