using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    
    /*********************
     Generation Parameters
    *********************/

    public GameObject mapNodePrefab;            // Prefab for mapnodes
    public Transform mapNodeContainer;          // Transform that will parent all created MapNodes
    public List<Transform> layerContainers;     // List of the transforms containing each layer

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
    private int currentLayerIndex;              // Current layer being operated on
    private MapNode firstNode;                  // First node to begin generation

    // Initialize data structures on wakeup
    void Awake() 
    { 
        layersList = new List<List<MapNode>>(); 
        layerContainers = new List<Transform>();
    }

    /*********************
     Main Functionality
    *********************/

    void Start() { DoGeneration(); }

    void Update()
    {
        foreach (List<MapNode> layer in layersList)
        {
            foreach (MapNode node in layer)
            {
                foreach (MapNode child in node.outNodes)
                    Debug.DrawLine(node.transform.position, child.transform.position, Color.green);
            }
        }
    }

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
    public void Initialize()
    {
        // Create parent container for mapnodes if none provided
        mapNodeContainer = mapNodeContainer == null ?  new GameObject("MapNode Container").transform : mapNodeContainer;

        // Configure randomness
        randomSeed = useSetSeed ? randomSeed : (int)System.DateTime.Now.Ticks;
        Random.InitState(randomSeed);

        ClearGenerationObjects();
        layersList.Add(new List<MapNode>());

        // Create first node
        firstNode = Instantiate(mapNodePrefab, mapNodeContainer).GetComponent<MapNode>();
        layersList[0].Add(firstNode);

    }

    // Clear previously generated objects and reset data structures
    void ClearGenerationObjects() 
    {
        // Destroy every game object and then clear references
        foreach (List<MapNode> layer in layersList)
        { 
            foreach (MapNode node in layer) { Destroy(node.gameObject); }
            layer.Clear(); 
        }
        layersList.Clear();
        numLayers           = 0;
        currentLayerIndex   = 0;

        foreach (Transform lc in layerContainers) { Destroy(lc.gameObject); }
        layerContainers.Clear();
    }

    /*********************
     Generation
    *********************/

    public void GenerateLayout() 
    { 
        // Generate layers between start and end
        while (numLayers < layersToGenerate)
        {
            List<MapNode> curLayer = layersList[currentLayerIndex];
            List<MapNode> nextLayer = new List<MapNode>();

            // Make first generation choice
            foreach (MapNode node in curLayer)
            {
                MakeGenerationChoice(node);
                if (node.choice == GenerationChoice.Forward) DoForward(node);
                else if (node.choice == GenerationChoice.Split) DoSplit(node);
            }
            // Prune invalid merges and force to forward if both directions invalid.
            PruneInvalidMergesToForward(curLayer);
            
            // Handle remaining merges. All are valid, so just create connections (and nodes, if needed)
            // Also, create the list for the next layer
            for (int i = 0; i < curLayer.Count; i++)
            {
                MapNode curNeighbour;
                MapNode node = curLayer[i];

                if ((node.choice & GenerationChoice.MergeBoth) == 0) 
                {
                    foreach (MapNode child in node.outNodes) 
                    nextLayer.Add(child);
                    continue;
                }

                // Left merge
                if ((node.choice & GenerationChoice.MergeLeft) != 0)
                {
                    curNeighbour = curLayer[i-1];
                    node.AddChildLeft(curNeighbour.GetRightmostChild());
                }

                // Right merge
                if ((node.choice & GenerationChoice.MergeRight) != 0)
                {
                    curNeighbour = curLayer[i+1];
                    MapNode lmChild = curNeighbour.GetLeftmostChild();
                    if (!lmChild)
                    {
                        lmChild = Instantiate(mapNodePrefab, mapNodeContainer).GetComponent<MapNode>();
                        nextLayer.Add(lmChild);
                    }
                    node.AddChildRight(lmChild);
                }
            }
            layersList.Add(nextLayer);
            numLayers++;
            currentLayerIndex++;
        }

        // Generate final layer and connect previous layer into it

        return ;
    }

    private void MakeGenerationChoice(MapNode node)
    {
        // TO DO: Make choice according to probabilities.
        // Where we draw these probabilities from is TBD...
        node.choice = GenerationChoice.MergeLeft;
        return;
    }

    private void DoForward(MapNode node)
    {
        MapNode newNode;
        newNode = Instantiate(mapNodePrefab, mapNodeContainer).GetComponent<MapNode>();
        newNode.branch = node.branch;
        node.AddChildRight(newNode);
    }

    private void DoSplit(MapNode node)
    {
        MapNode newNode;
        newNode = Instantiate(mapNodePrefab, mapNodeContainer).GetComponent<MapNode>();
        newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);        
        newNode = Instantiate(mapNodePrefab, mapNodeContainer).GetComponent<MapNode>();
        newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);
        // TO DO: Determine method of placing "set sequences"
    }

    // Prune invalid merge flags and set the node to go forward instead if both invalid
    private void PruneInvalidMergesToForward(List<MapNode> curLayer)
    {
        int ni = 0;
        foreach (MapNode node in curLayer)
        {   
            if ((node.choice & GenerationChoice.MergeBoth) == 0) continue;
            CheckLeft(node, curLayer, ni);
            CheckRight(node, curLayer, ni);

            // Check if fully pruned and set to forward
            if (node.choice == GenerationChoice.None)
            {
                node.choice = GenerationChoice.Forward;
                DoForward(node);
            }
            ni++;
        }
    }

    // Check for merge left flag and validity, and prune if invalid
    private void CheckLeft(MapNode node, List<MapNode> curLayer, int ni)
    {
        if ((node.choice & GenerationChoice.MergeLeft) != 0)
        {
            if (ni == 0) { node.choice &= GenerationChoice.MergeRight; return; }
            MapNode leftNeighbour = curLayer[ni - 1];

            // Check if neightbour has merge right flag
            if ((leftNeighbour.choice & GenerationChoice.MergeRight) != 0) return;

            // Check if neightbour has forward or split
            if ((leftNeighbour.choice & (GenerationChoice.Split | GenerationChoice.Forward)) != 0)
            {
                // Check roll for success on merging into the child branch
                if (Random.Range(0f, 1f) <= leftNeighbour.branchInProbability) return;
            }
            node.choice &= GenerationChoice.MergeRight;      
        }
    }

    // Check for merge right flag and validity, and prune if invalid
    private void CheckRight(MapNode node, List<MapNode> curLayer, int ni)
    {
        if ((node.choice & GenerationChoice.MergeRight) != 0)
        {
            if (ni == 0) { node.choice &= GenerationChoice.MergeLeft; return; }
            MapNode rightNeighbour = curLayer[ni + 1];

            // Check if neightbour has merge left flag
            if ((rightNeighbour.choice & GenerationChoice.MergeLeft) != 0) return;

            // Check if neightbour has forward or split
            if ((rightNeighbour.choice & (GenerationChoice.Split | GenerationChoice.Forward)) != 0)
            {
                // Check roll for success on merging into the child branch
                if (Random.Range(0f, 1f) <= rightNeighbour.branchInProbability) return;
            }
            node.choice &= GenerationChoice.MergeLeft;      
        }
    }

    /*********************
     Post-Processing
    *********************/

    // Visualize the result of generation
    public void DoVisualization() 
    {   
        int li = 0;
        foreach (List<MapNode> curLayer in layersList)
        {   
            Transform layerContainer = new GameObject("Layer" + li.ToString()).transform;
            layerContainer.SetParent(mapNodeContainer, false);
            layerContainer.Translate(Vector3.right * 4 * li);
            layerContainers.Add(layerContainer);
            int ni = 0;
            foreach (MapNode node in curLayer)
            {
                node.transform.SetParent(layerContainer, false);
                node.transform.Translate(Vector3.back * (curLayer.Count - 1) * 2 + Vector3.forward * 4 * ni);
                ni++;
            }
            li++;
        }
    }
}
