using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterType 
{
    None    = 0,
    Start   = 1,
    Shop    = 2,
    Loot    = 3,
    Dungeon = 4,
    Boss = 5
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
        visAlpha        = 1f;
        visual          = transform.Find("Cylinder");

        crimson         = new Color(0.8627452f/2, 0.07843138f/2, 0.2352941f/2, 1f);
        gold            = new Color(0.854902f, 0.6470588f, 0.1254902f, 1f);
        goldenRod       = new Color(1f, 0.8431373f, 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**********************************
    ********** Regeneration  **********
    **********************************/
    public void AddChildLeft(MapEncounter child)   { children.Insert(0, child); }
    public MapEncounter GetLeftmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[0];
    }
    public void AddChildRight(MapEncounter child)  { children.Add(child);       }
    public MapEncounter GetRightmostChild() 
    { 
        int n = children.Count;
        return n == 0 ? null : children[n-1];
    }
    public void SetEncounter(EncounterType enc)
    {
        encounter = enc;
        if (enc == EncounterType.Start) { SetVisColor(Color.blue); return; }
        if (enc == EncounterType.Shop) { SetVisColor(goldenRod); return; }
        if (enc == EncounterType.Loot){ SetVisColor(gold); return; }
        if (enc == EncounterType.Boss) { SetVisColor(crimson); return; }
        if (enc == EncounterType.Dungeon) { SetVisColor(Color.red); return; }
    }

    /**********************************
    ********** Visualization **********
    **********************************/
    public void UpdateAppearance()
    {
        Color newColor = new Color(visColor.r, visColor.g, visColor.b, visAlpha);
        visual.GetComponent<Renderer>().material.color = newColor;
    }
    public void SetVisAlpha(float a)
    {
        visAlpha = a;
        UpdateAppearance();
    }
    public void SetVisColor(Color c)
    {
        visColor = c;
        UpdateAppearance();
    }
}
