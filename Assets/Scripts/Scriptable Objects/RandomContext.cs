using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/RandomContext")]
public class RandomContext : ScriptableObject
{
    private System.Random rnd;
    public int seed;

    public void ResetContext()
    {
        rnd = new System.Random(seed);
    }
    public void ResetContext(int seed)
    {
        rnd = new System.Random(seed);
        this.seed = seed;
    }

    public int GetNext()
    {
        CheckForExistence();
        return rnd.Next();
    }

    public int GetNext(int upperBound)
    {
        CheckForExistence();
        return rnd.Next(upperBound);
    }

    public int GetNext(int lowerBound, int upperBound)
    {
        CheckForExistence();
        int val = rnd.Next(upperBound);
        return val + lowerBound;
    }

    public float GetNext(float upperBound)
    {
        CheckForExistence();
        return (float) (rnd.NextDouble() * upperBound);
    }

    public float GetNext(float lowerBound, float upperBound)
    {
        CheckForExistence();
        float val = GetNext(Mathf.Abs(upperBound - lowerBound));
        return val + lowerBound;
    }
    
    private void CheckForExistence()
    {
        rnd ??= new System.Random(seed);
    }
}
