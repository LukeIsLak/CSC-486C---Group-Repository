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
    public EncounterType encounter;

    // For navigation and visualization
    public bool isAccessible;
    public bool isCompleted;
    public bool isHovered;
    public bool isSelected;

    public List<MapEncounter> children;
    public Color visColor;
    public float visAlpha;
    public TraversableLayout traversableLayout;

    // Internal reference 
    private Transform visual;
    private Color crimson;
    private Color gold;
    private Color goldenRod;


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
        visual          = transform.Find("Cylinder");

        crimson         = new Color(0.8627452f/2, 0.07843138f/2, 0.2352941f/2, 1f);
        gold            = new Color(0.854902f, 0.6470588f, 0.1254902f, 1f);
        goldenRod       = new Color(1f, 0.8431373f, 0f, 1f);
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
        UpdateAppearance();
    }
    public void SetIsSelected(bool val)
    {
        isSelected = val;
        UpdateAppearance();
    }
    public void UpdateAppearance()
    {
        if (encounter == EncounterType.Start)     visColor = Color.blue;
        if (encounter == EncounterType.Merchant)  visColor = goldenRod;
        if (encounter == EncounterType.Treasure)  visColor = gold;
        if (encounter == EncounterType.Boss)      visColor = crimson;
        if (encounter == EncounterType.Dungeon)   visColor = Color.red;
        if (isSelected) visColor = Color.white;
        if (isCompleted) visColor = Color.green;

        if (!isAccessible && !isCompleted) {visAlpha = 0.1f;}
        else {visAlpha = 1.0f;}
        
        Color newColor = new Color(visColor.r, visColor.g, visColor.b, visAlpha);
        visual.GetComponent<Renderer>().material.color = newColor;
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
    }
}
