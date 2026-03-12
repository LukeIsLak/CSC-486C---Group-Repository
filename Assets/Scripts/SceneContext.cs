using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SceneType
{
    NodeTraversal,
    Combat,
    Merchant,
    DoNothing,
    ForceMouseOn

}
public class SceneContext : MonoBehaviour
{
    public SceneType sceneType;
    public bool doRefresh;
}
