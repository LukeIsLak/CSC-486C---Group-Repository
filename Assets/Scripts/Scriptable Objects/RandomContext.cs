using UnityEngine;

[CreateAssetMenu(menuName ="Data/RandomContext")]
public class RandomContext : ScriptableObject
{
    public Unity.Mathematics.Random rnd;
    public int seed;

    public void ResetContext()
    {
        if (seed == 0) Debug.Log("Seed 0 somehow.");
        rnd = new Unity.Mathematics.Random((uint)seed);
    }
    public void ResetContext(int seed)
    {
        this.seed = seed;
        ResetContext();
    }

}
