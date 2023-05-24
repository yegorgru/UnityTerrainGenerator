using UnityEngine;

public abstract class NoiseData : ScriptableObject
{
    public enum NoiseType
    {
        ValueNoise,
        PerlinNoise,
        VoronoiNoise,
        GridVoronoiNoise,
        ChebyshevVoronoiNoise,
        ManhattanVoronoiNoise
    }

    public enum ValueInterpolationType
    {
        Linear,
        Smooth
    }

    public enum VoronoiDistanceType
    {
        Warley,
        Chebyshev,
        Manhattan
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

    public virtual int GetPivotNumber()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual VoronoiDistanceType GetVoronoiDistanceType()
    {
        throw new System.Exception("Incorrect noise data provided");
    }

    public virtual ValueInterpolationType GetValueInterpolationType()
    {
        throw new System.Exception("Incorrect noise data provided");
    }
}
