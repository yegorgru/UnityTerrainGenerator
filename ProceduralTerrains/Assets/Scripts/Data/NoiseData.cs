using UnityEngine;

public abstract class NoiseData : ScriptableObject
{
    public enum NoiseType
    {
        ValueNoise,
        PerlinNoise,
    }

    public virtual NoiseType GetNoiseType()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual float GetNoiseScale()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual int GetNumberOctaves()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual float GetPersistance()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual float GetLacunarity()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual int GetSeed()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual Vector2 GetOffset()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual int GetGridFrequency()
    {
        throw new System.Exception("Incorrect noise data provided");
    }
}
