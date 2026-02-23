using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavMeshSurfaceConstructor : MonoBehaviour
{
    [Header("Source NavMesh Settings")]
    public LayerMask floorLayer;
    public LayerMask floorNavLayer;
    public float cellSize = 1f;
    public float navmeshHeightOffset = 0.02f;
    public string agentName;
    public int agentTypeID;

    [Header("Build Settings")]
    public bool buildOnStart = true;
    public bool rebuild;

    private NavMeshSurface surface;
    private MeshFilter mf;
    private MeshRenderer mr;

    public GameEvent doneNavMeshGeneration;

    HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

    public void Initialize() {
        if (buildOnStart) Build();
    }

    void Update() {
        if (rebuild) {
            rebuild = false;
            Build();
        }
    }

    public void Build() {
        int? id = GetNavMeshAgentID(agentName); agentTypeID = id ?? 0;
        agentTypeID = id ?? 0;

        CollectCells();
        Mesh mesh = GenerateMesh();

        SetupComponents();

        mf.sharedMesh = mesh;

        surface.agentTypeID = agentTypeID;
        surface.BuildNavMesh();

        doneNavMeshGeneration.Raise();
    }

    private void CollectCells() {
        occupied.Clear();

        Collider[] cols = FindObjectsOfType<Collider>();

        foreach (var c in cols)
        {
            if (((1 << c.gameObject.layer) & floorLayer.value) == 0) continue;

            Bounds b = c.bounds;

            int minX = Mathf.FloorToInt(b.min.x / cellSize);
            int maxX = Mathf.FloorToInt(b.max.x / cellSize);

            int minZ = Mathf.FloorToInt(b.min.z / cellSize);
            int maxZ = Mathf.FloorToInt(b.max.z / cellSize);

            for (int x = minX; x < maxX; x++) {
                for (int z = minZ; z < maxZ; z++) occupied.Add(new Vector2Int(x, z));
            }
        }
    }

    private Mesh GenerateMesh() {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        float y = GetHeight();

        foreach (Vector2Int cell in occupied) {
            // Only create faces exposed to empty space
            AddFaceIfEmpty(cell, Vector2Int.up, verts, tris, y);
            AddFaceIfEmpty(cell, Vector2Int.down, verts, tris, y);
            AddFaceIfEmpty(cell, Vector2Int.left, verts, tris, y);
            AddFaceIfEmpty(cell, Vector2Int.right, verts, tris, y);
        }

        Mesh m = new Mesh();
        m.name = "CombinedNavMesh";
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();

        return m;
    }

    private void AddFaceIfEmpty(Vector2Int cell, Vector2Int dir, List<Vector3> verts, List<int> tris, float y) {
        if (occupied.Contains(cell + dir)) return;

        Vector3 basePos = new Vector3(cell.x * cellSize, y, cell.y * cellSize);

        Vector3 right = new Vector3(cellSize, 0, 0);
        Vector3 forward = new Vector3(0, 0, cellSize);

        Vector3 v0, v1, v2, v3;

        if (dir == Vector2Int.up) {
            v0 = basePos + forward + right;
            v1 = basePos + forward;
            v2 = basePos;
            v3 = basePos + right;
        }
        else if (dir == Vector2Int.down) {
            v0 = basePos;
            v1 = basePos + right;
            v2 = basePos + forward + right;
            v3 = basePos + forward;
        }
        else if (dir == Vector2Int.left) {
            v0 = basePos + forward;
            v1 = basePos;
            v2 = basePos + right;
            v3 = basePos + forward + right;
        }
        else {
            v0 = basePos + right;
            v1 = basePos + forward + right;
            v2 = basePos + forward;
            v3 = basePos;
        }

        int start = verts.Count;

        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v3);

        tris.Add(start + 0);
        tris.Add(start + 1);
        tris.Add(start + 2);

        tris.Add(start + 0);
        tris.Add(start + 2);
        tris.Add(start + 3);
    }

    private float GetHeight() {
        foreach (var c in FindObjectsOfType<Collider>()) if (((1 << c.gameObject.layer) & floorLayer) != 0) return c.bounds.center.y + navmeshHeightOffset;
        return 0f;
    }

    private void SetupComponents() {
        mf = GetComponent<MeshFilter>();
        if (!mf) mf = gameObject.AddComponent<MeshFilter>();

        mr = GetComponent<MeshRenderer>();
        if (!mr) mr = gameObject.AddComponent<MeshRenderer>();

        mr.enabled = false;

        surface = GetComponent<NavMeshSurface>();
        if (!surface) surface = gameObject.AddComponent<NavMeshSurface>();

        surface.collectObjects = CollectObjects.All;
        surface.layerMask = floorNavLayer;
        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
    }

    private int? GetNavMeshAgentID(string name) {
        for (int i = 0; i < NavMesh.GetSettingsCount(); i++) { 
            var settings = NavMesh.GetSettingsByIndex(i); 
            if (name == NavMesh.GetSettingsNameFromID(settings.agentTypeID)) return settings.agentTypeID; 
        } 
        return null; 
    }

}