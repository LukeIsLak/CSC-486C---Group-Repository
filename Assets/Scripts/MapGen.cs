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
    public int maxDepth         = 5;            // Depth to generate until
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
        layerList.Clear();
        numLayers    = 0;
        currentLayer = 0;
    }

    /*********************
     Generation
    *********************/

    private void GenerateLayout() 
    { 
        /*
        For each node in each layer
        Make generation choice
        Split? No -> Continue? No -> Merge? No -> Continue.

        Iterate again to create next layer
        Repeat
        */
        
        while (numLayers)
        for (int i = 0; i < layers)

        return ;
    }

    private void MakeGenerationChoice(MapNode node)
    {
        // TO DO: Make choice according to probability distribution
        node.choice = GenerationChoice.Continue;
        return;
    }

    /*********************
     Visualization
    *********************/

    // Visualize the result generation
    private void DoVisualization() { return; }
}
