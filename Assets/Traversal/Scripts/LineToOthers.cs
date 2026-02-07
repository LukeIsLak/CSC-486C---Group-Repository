using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToOthers : MonoBehaviour
{
    private List<MapEncounter>  endPoints;          // Transforms to draw line towards
    private List<LineRenderer>  lineRends;            // All child game object w/ line renderers
    
    void Awake()
    {
        endPoints   = new List<MapEncounter>();
        lineRends   = new List<LineRenderer>();
    }

    public void SetOthers(List<MapEncounter> e)
    {
        ClearLines();
        endPoints = e;
        foreach (MapEncounter me in e)
        {
            GameObject line     = new GameObject("Line");
            line.transform.SetParent(transform);
            LineRenderer lr     = line.AddComponent<LineRenderer>();
            lr.material         = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth       = 0.05f;
            lr.endWidth         = 0.05f;
            lr.positionCount    = 2;
            lr.useWorldSpace    = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, me.transform.position);
            lineRends.Add(lr);
        }
    }

    public void UpdateLines()
    {
        for (int i = 0; i < lineRends.Count; i++)
        {
            MapEncounter me = endPoints[i];
            LineRenderer lr = lineRends[i];
            
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, me.transform.position);
        }
    }

    public void ClearLines()
    {
        foreach (LineRenderer lr in lineRends)
        {
            Destroy(lr.gameObject);
        }
        lineRends.Clear();
    }
}
