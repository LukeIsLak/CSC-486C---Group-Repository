using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(menuName = "Data/RandomContext")]
public class RandomContext : ScriptableObject
{
    [SerializeField] public uint seed = 1;
    [SerializeField] public uint state = 1;

    private Unity.Mathematics.Random rnd;

    public void ResetContext(uint newSeed)
    {
        seed = newSeed == 0 ? 1u : newSeed;
        rnd = new Unity.Mathematics.Random(seed);
        state = rnd.state;
    }

    public void RestoreState()
    {
        rnd = new Unity.Mathematics.Random(state);
    }

    void SaveState()
    {
        state = rnd.state;
    }

    // Int

    public int NextInt()
    {
        RestoreState();
        int v = rnd.NextInt();
        SaveState();
        return v;
    }

    public int NextInt(int max)
    {
        RestoreState();
        int v = rnd.NextInt(max);
        SaveState();
        return v;
    }

    public int NextInt(int min, int max)
    {
        RestoreState();
        int v = rnd.NextInt(min, max);
        SaveState();
        return v;
    }

    // Float

    public float NextFloat()
    {
        RestoreState();
        float v = rnd.NextFloat();
        SaveState();
        return v;
    }

    public float NextFloat(float max)
    {
        RestoreState();
        float v = rnd.NextFloat(max);
        SaveState();
        return v;
    }

    public float NextFloat(float min, float max)
    {
        RestoreState();
        float v = rnd.NextFloat(min, max);
        SaveState();
        return v;
    }

    // Boolean

    public bool NextBool()
    {
        RestoreState();
        bool v = rnd.NextBool();
        SaveState();
        return v;
    }
}