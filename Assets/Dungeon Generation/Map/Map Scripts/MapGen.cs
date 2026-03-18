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

public class MapGen : MonoBehaviour
{
    // Assign in prefab
    public GameObject MapNodePrefab;           // Prefab for MapNodes
    
    /*********************
     Generation Parameters
    *********************/


    [Header("Generation Parameters")]
    public int layersToGenerate = 5;            // Depth to generate until
    public int maxWidth         = 3;            // Maximum number of branches in a single layer.
    public int randomSeed       = 0;            // The random seed to use in generation
    public bool useSetSeed      = false;        // Whether not to use to provided seed
    public float complexity     = 0f;    // Chance of making an inexistent connection
    public float minWidthFraction = 0.5f;


    /*********************
     Data Structures
    *********************/
    public Transform MapNodeContainer;         // Transform that will parent all created MapNodes
    public List<Transform> layerContainers;     // List of the transforms containing each layer
    private List<List<MapNode>> layersList;     // List whose entries are lists of the nodes at each layer
    private int numLayers;                      // Current number of layers
    private int currentLayerIndex;              // Current layer being operated on
    private MapNode firstNode;                  // First node to begin generation
    private MapNode lastNode;                   // last node to end generation
    private List<GenOp> choices;                // ough

    // Initialize data structures on wakeup
    void Awake() 
    { 
        layersList      = new List<List<MapNode>>(); 
        layerContainers = new List<Transform>();
        choices         = new List<GenOp>();
    }

    /*********************
     Main Functionality
    *********************/

    void Update()
    {
        int li = 0;
        foreach (List<MapNode> layer in layersList)
        {
            foreach (MapNode node in layer)
            {
                foreach (MapNode child in node.outNodes)
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
    public List<List<MapNode>> DoGeneration()
    {
        Initialize();
        GenerateLayout();
        DoComplexity();
        DoVisualization();
        return layersList;
    }

    /*********************
     Initialization
    *********************/

    // Reset data structures, variables, random seed etc. for generation
    public void Initialize()
    {
        ClearGenerationObjects();
        // Create parent container for MapNodes if none provided
        MapNodeContainer = MapNodeContainer == null ?  new GameObject("MapNode Container").transform : MapNodeContainer;

        // Configure randomness
        randomSeed = useSetSeed ? randomSeed : (int)System.DateTime.Now.Ticks;
        Random.InitState(randomSeed);

        layersList.Add(new List<MapNode>());

        // Create first node
        firstNode = Instantiate(MapNodePrefab, MapNodeContainer).GetComponent<MapNode>();
        layersList[0].Add(firstNode);
    }

    // Clear previously generated objects and reset data structures
    public void ClearGenerationObjects() 
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
        
        if (MapNodeContainer != null) Destroy(MapNodeContainer.gameObject);
    }

    /*********************
     Generation
    *********************/

    public void GenerateLayout() 
    { 
        List<MapNode> curLayer;
        List<MapNode> nextLayer;

        // Generate layers between start and end
        while (numLayers < layersToGenerate)
        {
            curLayer = layersList[currentLayerIndex];
            
            // Set choices
            MakeGenerationChoices(curLayer);

            // Handle splits and forwards first
            foreach (MapNode node in curLayer)
            {
                if (node.choice == GenerationChoice.Forward) DoForward(node);
                else if (node.choice == GenerationChoice.Split) DoSplit(node);
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
    private void MakeGenerationChoices(List<MapNode> curLayer)
    {
        choices.Clear();
        choices.TrimExcess();

        int nextSize = DetermineNextWidth(curLayer.Count);

        // Determine imbalance of size and create merges or splits
        int diff = nextSize - curLayer.Count;
        GenOp surplusOp = diff < 0 ? GenOp.Merge : GenOp.Split;
        for (int i = 0; i < Mathf.Abs(diff); i++) choices.Add(surplusOp);
        int nodesMade = surplusOp == GenOp.Split? 2 * Mathf.Abs(diff) : Mathf.Abs(diff);
        int costLeft = nextSize - nodesMade;

        // Choose actions to consume remaining cost
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

        // Randomly poll from list and apply action to nodes in order
        int curNode = 0;
        while(choices.Count > 0)
        {
            int i       = Random.Range(0, choices.Count);
            GenOp op    = choices[i];
            choices.RemoveAt(i);
            if (op == GenOp.Forward)
            {
                curLayer[curNode].choice = GenerationChoice.Forward;
                curNode++;
                continue;
            }
            if (op == GenOp.Split)
            {
                curLayer[curNode].choice = GenerationChoice.Split;
                curNode++;
                continue;
            }
            curLayer[curNode].choice = GenerationChoice.MergeRight;
            curLayer[curNode+1].choice = GenerationChoice.MergeLeft;
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
        if (distanceToEnd < 15) curMax = (int)Mathf.Min(curMax, (int)Mathf.Pow(2, distanceToEnd));

        // Tend to be half full at least
        curMin      = Mathf.Max(curMin, (int)(maxWidth * minWidthFraction));
        // Clamp to max
        curMin      = Mathf.Min(curMax, curMin);    

        return Random.Range(curMin, curMax + 1);
    }

    private void DoForward(MapNode node)
    {
        MapNode newNode;
        newNode = Instantiate(MapNodePrefab, MapNodeContainer).GetComponent<MapNode>();
        // newNode.branch = node.branch;
        node.AddChildRight(newNode);
    }


    private void DoSplit(MapNode node)
    {
        MapNode newNode;
        newNode = Instantiate(MapNodePrefab, MapNodeContainer).GetComponent<MapNode>();
        // newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);        
        newNode = Instantiate(MapNodePrefab, MapNodeContainer).GetComponent<MapNode>();
        // newNode.branch = node.branch; // Same branch for now
        node.AddChildRight(newNode);
        // TO DO: Determine method of placing "set sequences"
    }

    private List<MapNode> DoMergesAndNextLayer(List<MapNode> curLayer)
    {
        List<MapNode> nextLayer = new List<MapNode>();
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
                    lmChild = Instantiate(MapNodePrefab, MapNodeContainer).GetComponent<MapNode>();

                    nextLayer.Add(lmChild);
                }
                node.AddChildRight(lmChild);
            }
        }        
        return nextLayer;
    }

    // Prune invalid merge flags and set the node to go forward instead if both invalid
    private void PruneInvalidToForward(List<MapNode> curLayer)
    {
        int ni = -1;
        foreach (MapNode node in curLayer)
        {   
            ni++;
            if ((node.choice & GenerationChoice.MergeBoth) == 0 && node.choice != GenerationChoice.None) continue;
            CheckLeft(node, curLayer, ni);
            CheckRight(node, curLayer, ni);

            // Check if fully pruned and set to forward
            if (node.choice == GenerationChoice.None)
            {
                node.choice = GenerationChoice.Split;
                DoForward(node);
            }
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
            if (ni == curLayer.Count - 1) { node.choice &= GenerationChoice.MergeLeft; return; }
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

    private void DoComplexity()
    {
        for (int l = 0; l < numLayers - 1; l++)
        {
            List<MapNode> curLayer = layersList[l];
            for (int i = 0; i < curLayer.Count - 1; i++)
            {
                MapNode curNode = curLayer[i];
                MapNode nextNode = curLayer[i + 1];
                MapNode curChild = curNode.GetRightmostChild();
                MapNode nextChild = nextNode.GetLeftmostChild();

                bool canConnect = curChild != nextChild;

                if (canConnect) 
                {
                    // Divide the choice to go either left or right
                    float r = Random.Range(0f, 1f);
                    if (r <= complexity/2)
                    {
                        nextNode.AddChildLeft(curChild);
                    }
                    else if (r <= complexity)
                    {
                        curNode.AddChildRight(nextChild);
                    }  
                }
            }
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
            layerContainer.SetParent(MapNodeContainer, false);
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