using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterType 
{
    None        = 0,
    Start       = 1,
    Merchant    = 2,
    Treasure    = 3,
    Dungeon     = 4,
    Boss        = 5
}

public class MapEncounter : MonoBehaviour
{
    private static int ACCESSIBLE   = 0;
    private static int INACCESIBLE  = 1;
    private static int COMPLETED    = 2;

    public GameObject   graphicsGO;
    public SpriteRenderer graphicsSR;

    [Header("Encounter Icons")]
    public List<Sprite> merchantAIC;
    public List<Sprite> treasureAIC;
    public List<Sprite> dungeonAIC;
    public List<Sprite> bossAIC;

    [Header("Encounter Information")]
    public EncounterType encounter;

    // For navigation and visualization
    public bool isAccessible;
    public bool isCompleted;
    public bool isHovered;
    public bool isSelected;
    public int layer;
    public int index;

    public List<MapEncounter> children;
    public Color visColor;
    public float visAlpha;
    public TraversableLayout traversableLayout;

    // Internal reference 
    private Color crimson;
    private Color gold;
    private Color goldenRod;

    private float hoverHeight   = 0.3f;
    private float desiredHeight = 0.1f;
    private float baseHeight    = 0.1f;

    void Awake()
    {
        encounter       = EncounterType.None;
        isAccessible    = false;
        isCompleted     = false;
        isHovered       = false;
        isSelected      = false;
        children        = new List<MapEncounter>();
        visColor        = Color.white;
        visAlpha        = 0.1f;

        crimson         = new Color(0.8627452f/2, 0.07843138f/2, 0.2352941f/2, 1f);
        gold            = new Color(0.854902f, 0.6470588f, 0.1254902f, 1f);
        goldenRod       = new Color(1f, 0.8431373f, 0f, 1f);

    }
    void Start()
    {
        graphicsSR = graphicsGO.GetComponent<SpriteRenderer>();
    }


    /**********************************
    ********** Regeneration  **********
    **********************************/

    public void AddChildLeft(MapEncounter child)   { children.Insert(0, child); MakeLines(); }
    public MapEncounter GetLeftmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[0];
    }
    public void AddChildRight(MapEncounter child)  { children.Add(child); MakeLines(); }
    public MapEncounter GetRightmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[n-1];
    }
    public void SetEncounter(EncounterType enc)
    {
        encounter = enc;
        UpdateAppearance();
    }


    /**********************************
    ********** Visualization **********
    **********************************/

    public void SetIsAccessible(bool val)
    {
        isAccessible = val;
        UpdateAppearance();
    }
    public void SetIsCompleted(bool val)
    {
        isCompleted = val;
        isAccessible = false;
        UpdateAppearance();
    }
    public void SetIsHovered(bool val)
    {
        isHovered = val;
        desiredHeight = val && !isSelected ? hoverHeight : baseHeight;
        UpdateAppearance();
    }
    public void SetIsSelected(bool val)
    {
        isSelected = val;
        desiredHeight = baseHeight;
        UpdateAppearance();
    }

    public void UpdateAppearance()
    {
        int i = INACCESIBLE;
        if (isCompleted) i = COMPLETED;
        else if (isAccessible) i = ACCESSIBLE;

        if (!graphicsSR) { Debug.Log("No sr"); }
        if (encounter == EncounterType.Merchant)
        {
            graphicsSR.sprite = merchantAIC[i];
        }
        if (encounter == EncounterType.Treasure)
        {
            graphicsSR.sprite = treasureAIC[i];
        }
        if (encounter == EncounterType.Dungeon)
        {
            graphicsSR.sprite = dungeonAIC[i];
        }
        if (encounter == EncounterType.Boss)
        {
            graphicsSR.sprite = bossAIC[i];
        }
        UpdateLines();
    }

    public void MakeLines()
    {
        transform.Find("LineCreator").GetComponent<LineToOthers>().SetOthers(children);
    }
    public void UpdateLines()
    {
        transform.Find("LineCreator").GetComponent<LineToOthers>().UpdateLines();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isHovered) traversableLayout?.ReceiveClick(this);
        float curHeight = graphicsGO.transform.localPosition[1];
        float interp = (desiredHeight - curHeight)/4f;
        if (Mathf.Abs(interp) < 0.01f)
        {
            graphicsGO.transform.localPosition = new Vector3(0f, desiredHeight, 0f);
        }
        else 
        {
            graphicsGO.transform.localPosition += new Vector3(0f, interp, 0f);
        }
    }
}
