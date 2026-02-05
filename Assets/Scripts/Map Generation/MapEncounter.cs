using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EncounterType 
{
    Shop,
    Loot,
    Boss,
    Dungeon,
    None
}

public class MapEncounter : MonoBehaviour
{
    public EncounterType encounter;

    // For navigation and visualization
    public bool isAccessible;
    public bool isCompleted;
    public bool isHovered;
    public bool isSelected;
    public List<MapNode> children;

    void Start()
    {
        encounter       = EncounterType.None;
        isAccessible    = false;
        isCompleted     = false;
        isHovered       = false;
        isSelected      = false;
        children        = new List<MapNode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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


    
}
