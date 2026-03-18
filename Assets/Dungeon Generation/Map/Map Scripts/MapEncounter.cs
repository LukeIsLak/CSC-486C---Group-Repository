using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEncounter : MonoBehaviour
{
    public GameObject   graphicsGO;
    public SpriteRenderer graphicsSR;
    public TraversableLayout traversableLayout;

    [Header("Encounter Information")]
    public EncounterInfo encounter;

    [Header("Navigation States")]
    public bool isAccessible;
    public bool isCompleted;
    public bool isHovered;
    public bool isSelected;
    public int layer;
    public int index;

    [Header("Data Structures")]
    public List<MapEncounter> parents;
    public List<MapEncounter> children;


    void Awake()
    {
        isAccessible    = false;
        isCompleted     = false;
        isHovered       = false;
        isSelected      = false;
        children        = new List<MapEncounter>();
    }
    void Start()
    {
        graphicsSR = graphicsGO.GetComponent<SpriteRenderer>();
    }


    /**********************************
    ********** Regeneration  **********
    **********************************/

    public void AddChildLeft(MapEncounter child)   { children.Insert(0, child); child.parents.Add(this); MakeLines(); }
    public MapEncounter GetLeftmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[0];
    }
    public void AddChildRight(MapEncounter child)  { children.Add(child); child.parents.Insert(0, this); MakeLines(); }
    public MapEncounter GetRightmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[n-1];
    }
    public void SetEncounter(EncounterInfo enc)
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
        UpdateAppearance();
    }
    public void SetIsSelected(bool val)
    {
        isSelected = val;
        UpdateAppearance();
    }

    public void UpdateAppearance()
    {
        UpdateLines();

        if (encounter == null) return;
        int i = encounter.IND_INACCESSIBLE;
        if (isCompleted) i = encounter.IND_COMPLETED;
        else if (isAccessible) i = isHovered ? encounter.IND_HOVERED : encounter.IND_ACCESSIBLE;

        if (encounter.stateSprites.Count >= i) graphicsSR.sprite = encounter.stateSprites[i];

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
        if (Input.GetMouseButtonDown(0) && isHovered) traversableLayout.ReceiveClick(this);
    }
}
