using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMouseInfo : MonoBehaviour
{
    public MapEncounter parent;
    void OnMouseEnter()
    {
        parent.SetIsHovered(true);
    }
    void OnMouseExit()
    {
        parent.SetIsHovered(false);
    }
}
