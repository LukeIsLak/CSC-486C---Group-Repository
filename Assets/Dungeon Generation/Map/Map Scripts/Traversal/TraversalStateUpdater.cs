using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalStateUpdater : MonoBehaviour
{
    // Start is called before the first frame update

    private int numToReveal;
    public LayoutData layoutData;

    void Awake()
    {
        // For now, reveal to ensure we can see 2 ahead of completed indices
        numToReveal = Mathf.Max(layoutData.completedIndices.Count + layoutData.lookAhead - layoutData.layersRevealed, 0);
    }

    void Start()
    {
        DoFogReceding();
    }
    public void DoFogReceding()
    {
        StartCoroutine(RevealNum());
    }

    IEnumerator RevealNum()
    {
        for (int i = 0; i < numToReveal; i++)
        {
            yield return new WaitForSeconds(1f);
            layoutData.AddRevealed(1);
        }
    }
}
