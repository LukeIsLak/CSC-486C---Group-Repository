using UnityEngine;

public abstract class Acquirable : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite icon;

    public abstract void Acquire();
}
