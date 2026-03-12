using UnityEngine;

public abstract class Acquirable : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    public string itemDescription;
    public Sprite icon;

    [Header("Selection Parameters")]
    public float selectionWeight = 1;

    public abstract void Acquire();

    public virtual string GetName()
    {
        return itemName;
    }

    public virtual string GetDescription()
    {
        return itemDescription;
    }
}
