using UnityEngine;

public class SetRenderQueue : MonoBehaviour
{
    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.material = new Material(sr.material); // creates instance
        sr.material.renderQueue = 2450;
    }
}