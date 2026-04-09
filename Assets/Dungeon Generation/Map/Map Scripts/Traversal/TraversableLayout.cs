using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TraversableLayout : MonoBehaviour
{
    [Header("Required Prefabs")]
    public GameObject layoutGeneratorPrefab;
    private MapGen layoutGenerator;    
    public GameObject mapEncounterPrefab;
    public GameObject playerOnMapPrefab;
    private GameObject playerOnMap;
    
    [Header("Data")]
    public EncounterInfo defaultEncounter;
    public LayoutData layoutData;
    public EncounterInfo firstEncounter;
    public EncounterInfo lastEncounter;
    public GameEvent ClickSelected;
    public bool respondToInputs;

    // Data Structures
    private List<List<MapEncounter>> mapLayers;
    private List<List<MapNode>> genLayers;
    public List<Transform> layerContainers;

    // Related to traversal
    public MapEncounter prevSelectedEncounter;
    public MapEncounter curSelectedEncounter;

    private bool visualize = true;
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
        layoutGenerator = Instantiate(layoutGeneratorPrefab, transform).GetComponent<MapGen>();
        layoutGenerator.layersToGenerate    = layoutData.depth;
        layoutGenerator.maxWidth            = layoutData.maxWidth;
        layoutGenerator.useSetSeed          = layoutData.useSeed;
        layoutGenerator.randomSeed          = layoutData.randomSeed;
        layoutGenerator.complexity          = layoutData.complexity;
        layoutGenerator.minWidthFraction    = layoutData.minWidthFraction;
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

    public void InitializeUninteractable(bool draw)
    {
        visualize = draw;
        // Reset to like-new
        DestroyEverything();
        respondToInputs = false;

        // Do layout generation
        layoutGenerator = Instantiate(layoutGeneratorPrefab, transform).GetComponent<MapGen>();
        layoutGenerator.layersToGenerate    = layoutData.depth;
        layoutGenerator.maxWidth            = layoutData.maxWidth;
        layoutGenerator.useSetSeed          = layoutData.useSeed;
        layoutGenerator.randomSeed          = layoutData.randomSeed;
        layoutGenerator.complexity          = layoutData.complexity;
        layoutGenerator.minWidthFraction    = layoutData.minWidthFraction;
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
    private List<List<MapEncounter>> LayoutToEncounters(List<List<MapNode>> genLayers)
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
            List<MapNode> genLayer     = genLayers[i];        // Current layer from genLayers
            List<MapEncounter> resLayer = result[i];           // Current layer from result
            nextLayer = new List<MapEncounter>();
            int childIndex      = 0;
            
            // Create next layer
            for (int j = 0; j < genLayer.Count; j++)
            {   
                MapNode curGen     = genLayer[j];
                MapEncounter curRes = resLayer[j];
                foreach (MapNode child in curGen.outNodes)
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
        
        // START ENCOUNTER
        mapLayers[0][0].SetEncounter(firstEncounter);


        // FORCED REST LAYERS
        PlaceRestEncounters(mapLayers);

        // FINAL ENCOUNTER
        mapLayers[mapLayers.Count -1][0].SetEncounter(lastEncounter);


        // EVERYTHING THAT ISN'T SET
        foreach (List<MapEncounter> layer in mapLayers)
        {   foreach (MapEncounter enc in layer)
            {
                // Skip first encounter and already set encounters
                if (enc.parents.Count == 0 || enc.encounter != null) continue;
                enc.encounter = ChooseEncounterFromWeights(enc.parents);
            }
        }
    }

    // Place rest encounters according to the distances between in layoutData
    private void PlaceRestEncounters(List<List<MapEncounter>> mapLayers)
    {
        if (layoutData.distancesBetween.Count == 0) return;
        int curIndex = 0;
        int curSep  = 0;

        // Handle each distance
        foreach (int sep in layoutData.distancesBetween)
        {
            curSep = sep;

            // Doesn't make sense to have 0 or negative sep
            if (curSep <= 0)
            {
                Debug.Log("sep negative or zero; skipping value");
            }

            // Increment current layer index
            curIndex += curSep;
            
            // No layers remaining
            if (curIndex >= mapLayers.Count)
            {
                Debug.Log("Sum of seps larger than map layers; finishing rest placement");
                return;
            }

            // No previous layer from layer 0
            if (curIndex == 0) continue;


            // Randomly select one child encounter to be a rest
            List<MapEncounter> curLayer = mapLayers[curIndex - 1];
            MakeRestEncounterLayerAtNext(curLayer);
        
        }
        
        // Check if last sep 0
        if (curSep <= 0)
        {
            Debug.Log("Last sep 0 but layers remain; placing no rests");
            return;
        }

        // If there's more layers than the sum of distancesBetween, use lastSep for the rest
        curIndex += curSep;
        while (curIndex < mapLayers.Count)
        {
            MakeRestEncounterLayerAtNext(mapLayers[curIndex - 1]);
            curIndex += curSep;
        }
        

    }

    private void MakeRestEncounterLayerAtNext(List<MapEncounter> curLayer)
    {
        foreach (MapEncounter mapEnc in curLayer)
        {
            // If rest in chidren, continue
            bool childIsRest = false;
            foreach (MapEncounter childEnc in mapEnc.children)
            {
                if (childEnc.encounter == layoutData.restEncounter)
                {
                    childIsRest = true;
                    break;
                }
            }
            if (childIsRest) continue;

            // Randomly select a child to become a rest encounter
            mapEnc.children[Random.Range(0, mapEnc.children.Count)].encounter = layoutData.restEncounter;
        } 
    }
    private EncounterInfo ChooseEncounterFromWeights(List<MapEncounter> parents)
    {
        Dictionary<EncounterInfo, float> weights = new Dictionary<EncounterInfo, float>();
        List<EncounterInfo> blacklist = new List<EncounterInfo>();

        // Sum probabilities for each encounter, or create entry if not yet tracked
        foreach (MapEncounter parent in parents)
        {
            foreach (EncounterWeight encProb in parent.encounter.encounterWeights)
            {
                EncounterInfo enc   = encProb.encounter;
                float weight        = encProb.weight;
                
                // If weight is negative, add to blacklist and continue
                if (weight < 0)
                {
                    blacklist.Add(enc);
                    continue;
                }

                if (weights.ContainsKey(enc))
                {
                    weights[enc] += weight;
                    continue;
                }
                weights[enc] = weight;
            }
        }

        // Determine sum of weights ignoring (and vetting) blacklisted entries
        // We have to do this because we will ignore an encounter if ANY parent
        // has a negative weight, not just if the sum of weights is negative.
        // This assumes that there is some "default" encounter.
        float weightSum = 0f;
        foreach (EncounterInfo key in blacklist)
            weights.Remove(key);

        foreach (float weight in weights.Values)
            weightSum += weight;
        
        if (weights.Count == 0 || weightSum == 0f)
        {
            Debug.Log("No weights available in parent nodes or sum of weights is zero");
            return defaultEncounter;
        }

        float r = Random.Range(0f, weightSum);
        foreach (var (key, value) in weights)
        {
            r -= value;
            if (r <= 0f) return key;
        }
        // We shouldn't get here. In fact, we won't. It can't happen.
        Debug.LogWarning("Unreachable code reached!");
        return defaultEncounter;
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
            { node.UpdateAppearance(visualize); }
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
        playerOnMap = Instantiate(playerOnMapPrefab, lastCompleted.transform.position, lastCompleted.transform.rotation, parent: transform);
        return lastCompleted;
    }

    // Return a list of 
    public List<(EncounterInfo encounter, int index)> GetNextEncounters(List<int> completedIndices)
    {
        List<(EncounterInfo encounter, int index)> result = new();
        int l = 0;
        foreach (int i in completedIndices)
        {
            mapLayers[l++][i].isCompleted = true;
            // Debug.Log(l.ToString() + i.ToString());
        }

        foreach (MapEncounter child in mapLayers[--l][completedIndices[completedIndices.Count-1]].children)
        {
            child.isAccessible = true;
            result.Add((child.encounter, child.index));
        }
        return result;
    }

    public void ReceiveClick(MapEncounter enc)
    {
        if (!respondToInputs) return;
        if (enc.isSelected)
        {
            ClickSelected.Raise();
            respondToInputs = false;
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
