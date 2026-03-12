using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(menuName = "Data/RandomContext")]
public class RandomContext : ScriptableObject
{
    [SerializeField] private uint seed = 1;
    [SerializeField] private uint state = 1;

    private Unity.Mathematics.Random rnd;

    void OnEnable()
    {
        if (state == 0)
            state = seed == 0 ? 1u : seed;

        rnd = new Unity.Mathematics.Random(state);
    }

    public void ResetContext(uint newSeed)
    {
        seed = newSeed == 0 ? 1u : newSeed;
        rnd = new Unity.Mathematics.Random(seed);
        state = rnd.state;
    }

    void SaveState()
    {
        state = rnd.state;
    }

    // Int

    public int NextInt()
    {
        int v = rnd.NextInt();
        SaveState();
        return v;
    }

    public int NextInt(int max)
    {
        int v = rnd.NextInt(max);
        SaveState();
        return v;
    }

    public int NextInt(int min, int max)
    {
        int v = rnd.NextInt(min, max);
        SaveState();
        return v;
    }

    // Float

    public float NextFloat()
    {
        float v = rnd.NextFloat();
        SaveState();
        return v;
    }

    public float NextFloat(float max)
    {
        float v = rnd.NextFloat(max);
        SaveState();
        return v;
    }

    public float NextFloat(float min, float max)
    {
        float v = rnd.NextFloat(min, max);
        SaveState();
        return v;
    }

    // Boolean

    public bool NextBool()
    {
        bool v = rnd.NextBool();
        SaveState();
        return v;
    }
}