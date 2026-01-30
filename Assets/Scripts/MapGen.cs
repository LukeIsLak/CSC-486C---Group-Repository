using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    
    /*********************
     Generation Parameters
    *********************/

    public GameObject mapNodePrefab;            // Prefab for mapnodes
    public GameObject mapNodeContainer;         // Transform that will parent all created MapNodes

    [Header("Generation Parameters")]
    public int layersToGenerate = 5;            // Depth to generate until
    public int maxWidth         = 3;            // Maximum number of branches in a single layer.
    public int randomSeed       = 0;            // The random seed to use in generation
    public bool useSetSeed      = false;        // Whether not to use to provided seed


    /*********************
     Data Structures
    *********************/

    private List<List<MapNode>> layersList;     // List whose entries are lists of the nodes at each layer
    private int numLayers;                      // Current number of layers
    private int currentLayer;                   // Current layer being operated on
    private MapNode firstNode;                  // First node to begin generation

    // Initialize data structures on wakeup
    void Awake() 
    { 
        layersList = new List<List<MapNode>>(); 
    }

    /*********************
     Main Functionality
    *********************/
    

    void Start() { DoGeneration(); }

    // Perform a compelete round of generation  
    void DoGeneration()
    {
        Initialize();
        GenerateLayout();
        DoVisualization();
    }

    /*********************
     Initialization
    *********************/

    // Reset data structures, variables, random seed etc. for generation
    void Initialize()
    {
        // Create parent container for mapnodes if none provided
        mapNodeContainer = new GameObject("MapNode Container");

        // Configure randomness
        randomSeed = useSetSeed ? randomSeed : (int)System.DateTime.Now.Ticks;
        Random.InitState(randomSeed);

        ClearGenerationObjects();
        layersList.Add(new List<MapNode>());

        // Create first node
        firstNode = Instantiate(mapNodePrefab, mapNodeContainer.transform).GetComponent<MapNode>();
        layersList[0].Add(firstNode);

    }

    // Clear previously generated objects and reset data structures
    void ClearGenerationObjects() 
    {
        // Destroy every game object and then clear references
        foreach (List<MapNode> layer in layersList)
        { 
            foreach (MapNode node in layer) { Destroy(node); }
            layer.Clear(); 
        }
        layersList.Clear();
        numLayers    = 0;
        currentLayer = 0;
    }

    /*********************
     Generation
    *********************/

    private void GenerateLayout() 
    { 
        
        // Generate layers between start and end
        while (numLayers < layersToGenerate)
        {
            List<MapNode> curNodesList = layersList[currentLayer];

            // Make first genertion choice
            foreach (MapNode node in curNodesList)
            {
                MakeGenerationChoice(node);
                if (node.choice = GenerationChoice.Forward) DoForward(node);
                else if (node.choice = GenerationChoice.Split) DoSplit(node);
            }
            PruneInvalidMerges(curNodesList);
            
            // Force fully pruned merges to forward
            foreach (MapNode node in curNodesList)
            {
                if (node.choice == GenerationChoice.None)
                {
                    node.choice = GenerationChoice.Forward;
                    DoForward(node);
                }
            }

            // Final pass. Connect / Create children and append to next layer list in order
            ni = 0;
            foreach (MapNode node in curNodesList)
            {

            }

            
        }

        // Generate final layer and connect previous layer into it

        return ;
    }

    private void MakeGenerationChoice(MapNode node)
    {
        // TO DO: Make choice according to probabilities
        node.choice = GenerationChoice.Forward;
        return;
    }

    private void PruneInvalidMerges(List<MapNode> curNodesList)
    {
        int ni = 0;
        foreach (MapNode node in curNodesList)
        {   
            if ((node.choice & GenerationChoice.MergeBoth) == 0) continue;

            // Left check
            if ((node.choice & GenerationChoice.MergeLeft) != 0)
            {
                if (ni == 0)  node.choice &= GenerationChoice.MergeRight;
                MapNode leftNeighbour = curNodesList[ni - 1];

                // Check if neightbour has merge right flag
                if ((leftNeighbour.choice & GenerationChoice.MergeRight) != 0) continue;

                // Check if neightbour has forward or split
                if ((leftNeighbour.choice & (GenerationChoice.Split | GenerationChoice.Forward)) != 0)
                {
                    // Check roll for success on merging into the child branch
                    if (Random.Range(0f, 1f) <= leftNeighbour.branchInProbability) continue;
                }
                node.choice &= GenerationChoice.MergeRight;
            }

            // Right check
            if ((node.choice & GenerationChoice.MergeLeft) != 0)
            {
                if (ni == curNodesList.Count - 1)  node.choice &= GenerationChoice.MergeLeft;
                MapNode rightNeighbour = curNodesList[ni + 1];

                // Check if neightbour has merge left flag
                if ((rightNeighbour.choice & GenerationChoice.MergeLeft) != 0) continue;

                // Check if neightbour has forward or split
                if ((rightNeighbour.choice & (GenerationChoice.Split | GenerationChoice.Forward)) != 0)
                {
                    // Check roll for success on merging into the child branch
                    if (Random.Range(0f, 1f) <= rightNeighbour.branchInProbability) continue;
                }
                node.choice &= GenerationChoice.MergeLeft;
            }
            ni++;
        }
    }

    private void DoForward(MapNode node)
    {
        return;
    }

    private void DoSplit(MapNode node)
    {
        return;
    }

    /*********************
     Visualization
    *********************/

    // Visualize the result generation
    private void DoVisualization() { return; }
}
