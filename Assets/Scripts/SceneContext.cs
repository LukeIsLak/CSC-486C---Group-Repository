using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SceneType
{
    NodeTraversal,
    Combat,
    Merchant
}
public class SceneContext : MonoBehaviour
{
    public SceneType sceneType;
}
