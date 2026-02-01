using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* https://stackoverflow.com/questions/273313/randomize-a-listt */

public enum GenOp
{
    None        = 0b0000,
    Merge       = 0b0001,
    Forward     = 0b0010,
    Split       = 0b0100
}

public class MapGen2 : MonoBehaviour
{
    
    /*********************
     Generation Parameters
    *********************/

    public GameObject MapNode2Prefab;            // Prefab for MapNode2s
    public Transform MapNode2Container;          // Transform that will parent all created MapNode2s
    public List<Transform> layerContainers;     // List of the transforms containing each layer

    [Header("Generation Parameters")]
    public int layersToGenerate = 5;            // Depth to generate until
    public int maxWidth         = 3;            // Maximum number of branches in a single layer.
    public int randomSeed       = 0;            // The random seed to use in generation
    public bool useSetSeed      = false;        // Whether not to use to provided seed


    /*********************
     Data Structures
    *********************/

    private List<List<MapNode2>> layersList;     // List whose entries are lists of the nodes at each layer
    private int numLayers;                      // Current number of layers
    private int currentLayerIndex;              // Current layer being operated on
    private MapNode2 firstNode;                  // First node to begin generation
    private MapNode2 lastNode;                   // last node to end generation

    // Initialize data structures on wakeup
    void Awake() 
    { 
        layersList = new List<List<MapNode2>>(); 
        layerContainers = new List<Transform>();
    }

    /*********************
     Main Functionality
    *********************/

    void Start() { DoGeneration(); }

    void Update()
    {
        int li = 0;
        foreach (List<MapNode2> layer in layersList)
        {
            foreach (MapNode2 node in layer)
            {
                foreach (MapNode2 child in node.outNodes)
                {
                    Color toUse;
                    if (li == 0) { toUse = Color.blue;}
                    else if (li == layersList.Count - 2) {toUse = Color.red;}
                    else {toUse = Color.green;}
                    Debug.DrawLine(node.transform.position, child.transform.position, toUse);
                }
            }
            li++;
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
        // Create parent container for MapNode2s if none provided
        MapNode2Container = MapNode2Container == null ?  new GameObject("MapNode2 Container").transform : MapNode2Container;

        // Configure randomness
        randomSeed = useSetSeed ? randomSeed : (int)System.DateTime.Now.Ticks;
        Random.InitState(randomSeed);

        ClearGenerationObjects();
        layersList.Add(new List<MapNode2>());

        // Create first node
        firstNode = Instantiate(MapNode2Prefab, MapNode2Container).GetComponent<MapNode2>();
        layersList[0].Add(firstNode);
    }

    // Clear previously generated objects and reset data structures
    void ClearGenerationObjects() 
    {
        // Destroy every game object and then clear references
        foreach (List<MapNode2> layer in layersList)
        { 
            foreach (MapNode2 node in layer) { Destroy(node.gameObject); }
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
        List<MapNode2> curLayer;
        List<MapNode2> nextLayer;

        // Generate layers between start and end
        while (numLayers < layersToGenerate)
        {
            curLayer = layersList[currentLayerIndex];
            
            // Set choices
            MakeGenerationChoice2s(curLayer);

            // Handle splits and forwards first
            foreach (MapNode2 node in curLayer)
            {
                if (node.choice == GenerationChoice2.Forward) DoForward(node);
                else if (node.choice == GenerationChoice2.Split) DoSplit(node);
            }

            nextLayer = DoMergesAndNextLayer(curLayer);
            layersList.Add(nextLayer);
            numLayers++;
            currentLayerIndex++;
        }

    }

    /*
    Set the generation choice of each node
    */
    private void MakeGenerationChoice2s(List<MapNode2> curLayer)
    {
        List<GenOp> choices = new List<GenOp>();

        int nextSize = DetermineNextWidth(curLayer.Count);

        // Determine imbalance of size and create merges or splits
        int diff = nextSize - curLayer.Count;
        GenOp surplusOp = diff < 0 ? GenOp.Merge : GenOp.Split;
        for (int i = 0; i < Mathf.Abs(diff); i++) choices.Add(surplusOp);
        int nodesMade = surplusOp == GenOp.Split? 2 * Mathf.Abs(diff) : Mathf.Abs(diff);
        int costLeft = nextSize - nodesMade;

        while (costLeft > 0)
        {
            // If we can do a split + merge AND we roll it, do it.
            if (costLeft > 2 && Random.Range(0f, 1f) < 0.5f)
            {
                choices.Add(GenOp.Split);
                choices.Add(GenOp.Merge);
                costLeft -= 3;
                continue;
            }
            choices.Add(GenOp.Forward);
            costLeft--;
        }
        int n = choices.Count;
        while (n > 1) 
        {
            n--;
            int k = Random.Range(0, n);
            GenOp value = choices[k];
            choices[k] = choices[n];
            choices[n] = value;
        }

        // Now iterate and apply generation choices appropriately
        int curNode = 0;
        foreach (GenOp op in choices)
        {
            if (op == GenOp.Forward)
            {
                curLayer[curNode].choice = GenerationChoice2.Forward;
                curNode++;
                continue;
            }
            if (op == GenOp.Split)
            {
                curLayer[curNode].choice = GenerationChoice2.Split;
                curNode++;
                continue;
            }
            curLayer[curNode].choice = GenerationChoice2.MergeRight;
            curLayer[curNode+1].choice = GenerationChoice2.MergeLeft;
            curNode += 2;
        }
    }

    // Determine the size of the next layer
    private int DetermineNextWidth(int curSize)
    {
        // Mathematical upper and lower bounds
        int curMax  = Mathf.Min(curSize * 2, maxWidth);
        int curMin  = (curSize + curSize % 2) / 2;

        // Force convergence towards a single point
        int distanceToEnd = layersToGenerate - layersList.Count;
        curMax      = Mathf.Min(curMax, (int)Mathf.Pow(2, distanceToEnd));

        // Tend to be half full at least
        curMin      = Mathf.Max(curMin, (maxWidth + maxWidth % 2)/2);
        // Clamp to max
        curMin      = Mathf.Min(curMax, curMin);    

        return Random.Range(curMin, curMax + 1);
    }

    private void DoForward(MapNode2 node)
    {
        MapNode2 newNode;
        newNode = Instantiate(MapNode2Prefab, MapNode2Container).GetComponent<MapNode2>();
        newNode.branch = node.branch;
        node.AddChildRight(newNode);
    }


    private void DoSplit(MapNode2 node)
    {
        MapNode2 newNode;
        newNode = Instantiate(MapNode2Prefab, MapNode2Container).GetComponent<MapNode2>();
        newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);        
        newNode = Instantiate(MapNode2Prefab, MapNode2Container).GetComponent<MapNode2>();
        newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);
        // TO DO: Determine method of placing "set sequences"
    }

    private List<MapNode2> DoMergesAndNextLayer(List<MapNode2> curLayer)
    {
        List<MapNode2> nextLayer = new List<MapNode2>();
        for (int i = 0; i < curLayer.Count; i++)
        {
            MapNode2 curNeighbour;
            MapNode2 node = curLayer[i];

            if ((node.choice & GenerationChoice2.MergeBoth) == 0) 
            {
                foreach (MapNode2 child in node.outNodes) 
                nextLayer.Add(child);
                continue;
            }

            // Left merge
            if ((node.choice & GenerationChoice2.MergeLeft) != 0)
            {
                curNeighbour = curLayer[i-1];
                node.AddChildLeft(curNeighbour.GetRightmostChild());
            }

            // Right merge
            if ((node.choice & GenerationChoice2.MergeRight) != 0)
            {
                curNeighbour = curLayer[i+1];
                MapNode2 lmChild = curNeighbour.GetLeftmostChild();
                if (!lmChild)
                {
                    lmChild = Instantiate(MapNode2Prefab, MapNode2Container).GetComponent<MapNode2>();
                    nextLayer.Add(lmChild);
                }
                node.AddChildRight(lmChild);
            }
        }        
        return nextLayer;
    }

    // Prune invalid merge flags and set the node to go forward instead if both invalid
    private void PruneInvalidToForward(List<MapNode2> curLayer)
    {
        int ni = -1;
        foreach (MapNode2 node in curLayer)
        {   
            ni++;
            if ((node.choice & GenerationChoice2.MergeBoth) == 0 && node.choice != GenerationChoice2.None) continue;
            CheckLeft(node, curLayer, ni);
            CheckRight(node, curLayer, ni);

            // Check if fully pruned and set to forward
            if (node.choice == GenerationChoice2.None)
            {
                node.choice = GenerationChoice2.Split;
                DoForward(node);
            }
        }
    }

    // Check for merge left flag and validity, and prune if invalid
    private void CheckLeft(MapNode2 node, List<MapNode2> curLayer, int ni)
    {
        if ((node.choice & GenerationChoice2.MergeLeft) != 0)
        {
            if (ni == 0) { node.choice &= GenerationChoice2.MergeRight; return; }
            MapNode2 leftNeighbour = curLayer[ni - 1];

            // Check if neightbour has merge right flag
            if ((leftNeighbour.choice & GenerationChoice2.MergeRight) != 0) return;

            // Check if neightbour has forward or split
            if ((leftNeighbour.choice & (GenerationChoice2.Split | GenerationChoice2.Forward)) != 0)
            {
                // Check roll for success on merging into the child branch
                if (Random.Range(0f, 1f) <= leftNeighbour.branchInProbability) return;
            }
            node.choice &= GenerationChoice2.MergeRight;      
        }
    }

    // Check for merge right flag and validity, and prune if invalid
    private void CheckRight(MapNode2 node, List<MapNode2> curLayer, int ni)
    {
        if ((node.choice & GenerationChoice2.MergeRight) != 0)
        {   
            if (ni == curLayer.Count - 1) { node.choice &= GenerationChoice2.MergeLeft; return; }
            MapNode2 rightNeighbour = curLayer[ni + 1];

            // Check if neightbour has merge left flag
            if ((rightNeighbour.choice & GenerationChoice2.MergeLeft) != 0) return;

            // Check if neightbour has forward or split
            if ((rightNeighbour.choice & (GenerationChoice2.Split | GenerationChoice2.Forward)) != 0)
            {
                // Check roll for success on merging into the child branch
                if (Random.Range(0f, 1f) <= rightNeighbour.branchInProbability) return;
            }
            node.choice &= GenerationChoice2.MergeLeft;      
        }
    }

    /*********************
     Post-Processing
    *********************/

    // Visualize the result of generation
    public void DoVisualization() 
    {   
        int li = 0;
        foreach (List<MapNode2> curLayer in layersList)
        {   
            Transform layerContainer = new GameObject("Layer" + li.ToString()).transform;
            layerContainer.SetParent(MapNode2Container, false);
            layerContainer.Translate(Vector3.right * 4 * li);
            layerContainers.Add(layerContainer);
            int ni = 0;
            foreach (MapNode2 node in curLayer)
            {
                node.transform.SetParent(layerContainer, false);
                node.transform.Translate(Vector3.back * (curLayer.Count - 1) * 2 + Vector3.forward * 4 * ni);
                ni++;
            }
            li++;
        }
    }
}