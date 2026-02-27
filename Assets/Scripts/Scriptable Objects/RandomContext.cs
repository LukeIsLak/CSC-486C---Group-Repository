using UnityEngine;

[CreateAssetMenu(menuName ="Data/RandomContext")]
public class RandomContext : ScriptableObject
{
    public Unity.Mathematics.Random rnd;
    public int seed = 1;

    public void ResetContext()
    {
        if (seed == 0) seed = 1;
        rnd = new Unity.Mathematics.Random((uint)seed);
    }
    public void ResetContext(int seed)
    {
        this.seed = seed;
        ResetContext();
    }

}
