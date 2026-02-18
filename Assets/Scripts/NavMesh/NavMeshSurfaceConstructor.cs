using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

// XXX this is blocked until rooms are considered seperate meshes, since I need the meshes to be convesh mesh colliders
// for this to work. I have other things I can do in the meantime.

public class NavMeshSurfaceConstructor : MonoBehaviour
{
    [Header("Auto Collect")]
    public LayerMask componentLayer;

    [Header("Link Settings")]
    public float maxLinkDistance = 3f;
    public float linkWidth = 2f;
    public float surfaceInset = 0.15f; // push endpoints onto navmesh
    public bool bidirectional = true;

    private List<Collider> components = new List<Collider>();

    void Start()
    {
        CollectComponents();
        CreateLinks();
    }

    private void CollectComponents() {
        components.Clear();
        Collider[] all = FindObjectsOfType<Collider>();
        foreach (Collider col in all) if (((1 << col.gameObject.layer) & componentLayer) != 0) components.Add(col);
    }

    void CreateLinks() {
        for (int i = 0; i < components.Count; i++) {
            for (int j = i + 1; j < components.Count; j++) {
                Collider a = components[i];
                Collider b = components[j];

                // TODO: LK - Eventually make this consider the room center so we don't get diagonal rooms connecting
                Vector3 pointA = a.ClosestPoint(b.transform.position);
                Vector3 pointB = b.ClosestPoint(pointA);

                float dist = Vector3.Distance(pointA, pointB);

                if (dist > maxLinkDistance) continue;

                CreateGapLink(pointA, pointB, a.name, b.name);
            }
        }
    }

    void CreateGapLink(Vector3 pointA, Vector3 pointB, string nameA, string nameB) {
        Vector3 dir = (pointB - pointA).normalized;

        /*Push endpoints slightly into each surface so they are on the navmesh*/
        Vector3 start = pointA + dir * surfaceInset;
        Vector3 end   = pointB - dir * surfaceInset;

        /*Midpoint of the closest points becomes link object position*/
        Vector3 mid = (start + end) * 0.5f;

        GameObject linkObj = new GameObject($"NavMeshLink_{nameA}_{nameB}");

        linkObj.transform.parent = transform;
        linkObj.transform.position = mid;

        /*Link should face the direction of the gap*/
        linkObj.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        NavMeshLink link = linkObj.AddComponent<NavMeshLink>();

        link.startPoint = linkObj.transform.InverseTransformPoint(start);
        link.endPoint = linkObj.transform.InverseTransformPoint(end);

        link.width = linkWidth;
        link.bidirectional = bidirectional;

        Debug.DrawLine(start, end, Color.green, 20f);
    }
}