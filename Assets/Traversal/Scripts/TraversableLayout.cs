using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableLayout : MonoBehaviour
{
    [Header("Required Prefabs")]
    public GameObject layoutGeneratorPrefab;
    private MapGen2 layoutGenerator;    
    public GameObject mapEncounterPrefab;
    public GameObject playerOnMapPrefab;
    private GameObject playerOnMap;
    
    [Header("Data")]
    public LayoutData layoutData;
    public EncounterInfo firstEncounter;
    public EncounterInfo lastEncounter;
    public GameEvent ClickSelected;

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
    ********* Layout Creation *********
    **********************************/
    
    
    public void Initialize()
    {
        // Reset to like-new
        DestroyEverything();

        // Do layout generation
        layoutGenerator = Instantiate(layoutGeneratorPrefab, transform).GetComponent<MapGen2>();
        layoutGenerator.layersToGenerate    = layoutData.depth;
        layoutGenerator.maxWidth            = layoutData.maxWidth;
        layoutGenerator.useSetSeed          = layoutData.useSeed;
        layoutGenerator.randomSeed          = layoutData.randomSeed;
        layoutGenerator.complexity          = layoutData.complexity;
        genLayers = layoutGenerator.DoGeneration();

        // Do conversion to encounters, destroy generator
        mapLayers = LayoutToEncounters(genLayers);
        layoutGenerator.ClearGenerationObjects();
        Destroy(layoutGenerator.gameObject);

        // Run encounter placement algorithm
        PlaceEncounters(mapLayers);

        // Physical positioning
        DoLayerPlacement(mapLayers);
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
        newChild.layer = 0;
        newChild.index = 0;

        nextLayer.Add(newChild);
        result.Add(nextLayer);

        for (int i = 0; i < genLayers.Count - 1; i++)
        {   
            List<MapNode2> genLayer     = genLayers[i];        // Current layer from genLayers
            List<MapEncounter> resLayer = result[i];           // Current layer from result
            nextLayer = new List<MapEncounter>();
            int childIndex      = 0;
            
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
                        if (ci != -1) { curRes.AddChildRight(resLayer[j-1].children[ci]); continue;}
                    }

                    // Child doesn't exist yet, so add right (we are going l -> r)
                    newChild = Instantiate(mapEncounterPrefab, transform).GetComponent<MapEncounter>();
                    newChild.traversableLayout  = this;
                    // Debug.Log("Layer " + i.ToString() + childIndex.ToString());
                    newChild.layer              = i + 1;
                    newChild.index              = childIndex++;
                    curRes.AddChildRight(newChild);
                    nextLayer.Add(newChild);
                }
            }
            result.Add(nextLayer);
        }
        return result;
    }

    /**********************************
    ******* Encounter Placement *******
    **********************************/

    private void PlaceEncounters(List<List<MapEncounter>> mapLayers)
    {
        if (mapLayers.Count == 0) return;
        mapLayers[0][0].SetEncounter(firstEncounter);
        foreach (List<MapEncounter> layer in mapLayers)
        {   foreach (MapEncounter enc in layer)
            {
                if (enc.parents.Count == 0) continue; // For the first node.
                enc.encounter = ChooseEncounterFromWeights(enc.parents);
            }
        }
        mapLayers[mapLayers.Count -1][0].SetEncounter(lastEncounter);
    }

    private EncounterInfo ChooseEncounterFromWeights(List<MapEncounter> parents)
    {
        Dictionary<EncounterInfo, float> weights = new Dictionary<EncounterInfo, float>();
        float weightSum = 0f; // Save a loop by keeping track of this as we update the dictionary

        // Sum probabilities for each encounter, or create entry if not yet tracked
        foreach (MapEncounter parent in parents)
        {
            foreach (EncounterWeight encProb in parent.encounter.encounterWeights)
            {
                EncounterInfo enc   = encProb.encounter;
                float weight        = encProb.weight;
                weightSum           += weight;

                if (weights.ContainsKey(enc))
                {
                    weights[enc] += weight;
                    continue;
                }
                weights[enc] = weight;
            }
        }

        if (weights.Count == 0 || weightSum == 0f)
        {
            Debug.Log("No weights available in parent nodes or sum of weights is zero");
            return null;
        }

        float r = Random.Range(0f, weightSum);
        foreach (var (key, value) in weights)
        {
            r -= value;
            if (r <= 0f) return key;
        }
        // We shouldn't get here.
        return null;
    }


    /**********************************
    ************ Traversal ************
    **********************************/

    // Player prefabs in their own location
    private void DoLayerPlacement(List<List<MapEncounter>> mapLayers)
    {
        int li = 0;
        foreach (List<MapEncounter> curLayer in mapLayers)
        {   
            Transform layerContainer = new GameObject("Layer" + li.ToString()).transform;
            layerContainer.SetParent(transform, false);
            layerContainer.Translate(Vector3.back * layoutData.layerDistance * li);
            layerContainers.Add(layerContainer);
            int ni = 0;
            foreach (MapEncounter node in curLayer)
            {
                int sep = layoutData.encounterSep;
                node.transform.SetParent(layerContainer, false);
                node.transform.Translate(Vector3.left * (curLayer.Count - 1) * sep/2 + Vector3.right * sep * ni);
                ni++;
            }
            li++;
        }
        UpdateAppearance();
    }

    // Update the appearance of the map
    public void UpdateAppearance()
    { foreach (List<MapEncounter> curLayer in mapLayers)
        {   foreach (MapEncounter node in curLayer)
            { node.UpdateAppearance(); }
        }
    }

    // Progress given the indices of nodes completed at each layer
    public MapEncounter DoProgress(List<int> completedIndices)
    {
        int l = 0;
        foreach (int i in completedIndices)
        {
            mapLayers[l++][i].isCompleted = true;
            // Debug.Log(l.ToString() + i.ToString());
        }

        foreach (MapEncounter child in mapLayers[--l][completedIndices[completedIndices.Count-1]].children)
        {
            child.isAccessible = true;
        }

        UpdateAppearance();
        // Check for finish here!
        // if (prevselectedencounter = finalencounter)...
        MapEncounter lastCompleted = mapLayers[l][completedIndices[completedIndices.Count-1]];
        playerOnMap = Instantiate(playerOnMapPrefab, lastCompleted.transform.position, lastCompleted.transform.rotation);
        return lastCompleted;
    }

    public void ReceiveClick(MapEncounter enc)
    {
        if (enc.isSelected)
        {
            ClickSelected.Raise();
            return;
        }
        if (enc.isAccessible)
        {
            // Debug.Log(enc.encounter);
            curSelectedEncounter?.SetIsSelected(false);
            curSelectedEncounter = enc;
            playerOnMap.transform.position = curSelectedEncounter.transform.position;
            enc.SetIsSelected(true);
        }
    }

    void Update()
    {
        /*
        foreach (List<MapEncounter> layer in mapLayers)
        {   foreach (MapEncounter node in layer)
            {   foreach (MapEncounter child in node.children)
                {
                    Debug.DrawLine(node.transform.position, child.transform.position, Color.white);
                }
            }
        }
        */
    }
}
