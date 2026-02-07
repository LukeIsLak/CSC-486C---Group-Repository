using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableLayout : MonoBehaviour
{
    [Header("Required Prefabs")]
    public GameObject layoutGeneratorPrefab;
    private MapGen2 layoutGenerator;    
    public GameObject mapEncounterPrefab;
    
    
    [Header("Generation Parameters")]
    public int depth;
    public int maxWidth;
    public int randomSeed;
    public bool useSeed;

    // Data Structures
    private List<List<MapEncounter>> mapLayers;
    private List<List<MapNode2>> genLayers;
    public List<Transform> layerContainers;

    // Related to traversal
    public MapEncounter prevSelectedEncounter;
    public MapEncounter curSelectedEncounter;

    void Awake()
    {
        mapLayers = new List<List<MapEncounter>>();
        layerContainers = new List<Transform>();
    }

    /**********************************
    ********** PRE-TRAVERSAL **********
    **********************************/
    
    
    public void Initialize()
    {
        // Reset to like-new
        DestroyEverything();

        // Do layout generation
        layoutGenerator = Instantiate(layoutGeneratorPrefab, transform).GetComponent<MapGen2>();
        layoutGenerator.layersToGenerate    = depth;
        layoutGenerator.maxWidth            = maxWidth;
        layoutGenerator.useSetSeed          = useSeed;
        layoutGenerator.randomSeed          = randomSeed;
        genLayers = layoutGenerator.DoGeneration();

        // Do conversion to encounters, destroy generator
        mapLayers = LayoutToEncounters(genLayers);
        layoutGenerator.ClearGenerationObjects();
        Destroy(layoutGenerator.gameObject);

        // Run encounter placement algorithm
        PlaceEncounters(mapLayers);

        // Physical positioning
        DoLayerPlacement(mapLayers);

        InitTraversal();
    }

    // Reset as if never used
    public void DestroyEverything()
    {
        foreach (List<MapEncounter> layer in mapLayers)
        { 
            foreach (MapEncounter node in layer) { Destroy(node.gameObject); }
            layer.Clear(); 
        }
        mapLayers.Clear();

        foreach (Transform lc in layerContainers) { Destroy(lc.gameObject); }
        layerContainers.Clear();
        prevSelectedEncounter = null;
        curSelectedEncounter = null;
    }

    // Convert generated layout into structure of encounter nodes
    private List<List<MapEncounter>> LayoutToEncounters(List<List<MapNode2>> genLayers)
    {
        List<List<MapEncounter>> result = new List<List<MapEncounter>>();
        List<MapEncounter> nextLayer = new List<MapEncounter>();
        if (genLayers.Count == 0) return result;
        MapEncounter newChild = Instantiate(mapEncounterPrefab, transform).GetComponent<MapEncounter>();
        newChild.traversableLayout = this;

        nextLayer.Add(newChild);
        result.Add(nextLayer);

        for (int i = 0; i < genLayers.Count - 1; i++)
        {   
            List<MapNode2> genLayer     = genLayers[i];        // Current layer from genLayers
            List<MapEncounter> resLayer = result[i];           // Current layer from result
            nextLayer = new List<MapEncounter>();
            
            // Create next layer
            for (int j = 0; j < genLayer.Count; j++)
            {   
                MapNode2 curGen     = genLayer[j];
                MapEncounter curRes = resLayer[j];
                foreach (MapNode2 child in curGen.outNodes)
                {
                    if (j != 0)
                    {
                        // If the genNode is contained already in the left neighbour,
                        // the corresponding resNode will have the child to add at the same index.
                        int ci = genLayer[j-1].outNodes.IndexOf(child);
                        if (ci != -1) { curRes.AddChildLeft(resLayer[j-1].children[ci]); continue;}
                    }

                    // Child doesn't exist yet, so add right (we are going l -> r)
                    newChild = Instantiate(mapEncounterPrefab, transform).GetComponent<MapEncounter>();
                    newChild.traversableLayout = this;
                    curRes.AddChildRight(newChild);
                    nextLayer.Add(newChild);
                }
            }
            result.Add(nextLayer);
        }
        return result;
    }

    // Decide an encounter for each node
    private void PlaceEncounters(List<List<MapEncounter>> mapLayers)
    {
        if (mapLayers.Count == 0) return;
        mapLayers[0][0].SetEncounter(EncounterType.Start);
        mapLayers[mapLayers.Count -1][0].SetEncounter(EncounterType.Boss);
        foreach (List<MapEncounter> layer in mapLayers)
        {   foreach (MapEncounter enc in layer)
            {
                if (enc.encounter != EncounterType.None) continue;
                enc.SetEncounter((EncounterType)Random.Range(2, 5));
            }
        }
    }

    // Player prefabs in their own location
    private void DoLayerPlacement(List<List<MapEncounter>> mapLayers)
    {
        int li = 0;
        foreach (List<MapEncounter> curLayer in mapLayers)
        {   
            Transform layerContainer = new GameObject("Layer" + li.ToString()).transform;
            layerContainer.SetParent(transform, false);
            layerContainer.Translate(Vector3.right * 4 * li);
            layerContainers.Add(layerContainer);
            int ni = 0;
            foreach (MapEncounter node in curLayer)
            {
                node.transform.SetParent(layerContainer, false);
                node.transform.Translate(Vector3.back * (curLayer.Count - 1) * 2 + Vector3.forward * 4 * ni);
                ni++;
            }
            li++;
        }
    }


    /**********************************
    ************ Traversal ************
    **********************************/

    public void InitTraversal()
    {
        if (mapLayers.Count == 0) return;
        prevSelectedEncounter = mapLayers[0][0];
        prevSelectedEncounter.SetIsCompleted(true);
        foreach (MapEncounter child in prevSelectedEncounter.children)
        {
            child.SetIsAccessible(true);
        }
    }

    public void DoProgress()
    {
        if (!curSelectedEncounter) return;
        curSelectedEncounter.SetIsCompleted(true);

        foreach (MapEncounter child in prevSelectedEncounter.children)
        {
            child.SetIsAccessible(false);
        }
        foreach (MapEncounter child in curSelectedEncounter.children)
        {
            child.SetIsAccessible(true);
        }
        prevSelectedEncounter = curSelectedEncounter;
        curSelectedEncounter = null;

        // Check for finish here!
        // if (prevselectedencounter = finalencounter)...
    }

    public void ReceiveClick(MapEncounter enc)
    {
        if (enc.isAccessible)
        {
            Debug.Log(enc.encounter);
            curSelectedEncounter?.SetIsSelected(false);
            curSelectedEncounter = enc;
            enc.SetIsSelected(true);
        }
    }

    void Update()
    {
        foreach (List<MapEncounter> layer in mapLayers)
        {   foreach (MapEncounter node in layer)
            {   foreach (MapEncounter child in node.children)
                {
                    Debug.DrawLine(node.transform.position, child.transform.position, Color.white);
                }
            }
        }
    }
}
