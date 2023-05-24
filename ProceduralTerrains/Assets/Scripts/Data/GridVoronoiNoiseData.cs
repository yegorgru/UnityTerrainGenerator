using UnityEngine;

[CreateAssetMenu]
public class GridVoronoiNoiseData : NoiseData
{
    public int seed;
    public int gridFrequency;
    [Range(0f, 1f)]
    public float persistance;
    public int numberOctaves;
    public VoronoiDistanceType distanceType;

    private void OnValidate()
    {
        gridFrequency = Mathf.Max(1, gridFrequency);
        numberOctaves = Mathf.Max(numberOctaves, 1);
    }

    public override NoiseType GetNoiseType()
    {
        return NoiseType.GridVoronoiNoise;
    }

    public override int GetSeed()
    {
        return seed;
    }

    public override int GetGridFrequency()
    {
        return gridFrequency;
    }

    public override int GetNumberOctaves()
    {
        return numberOctaves;
    }

    public override float GetPersistance()
    {
        return persistance;
    }

    public override VoronoiDistanceType GetVoronoiDistanceType()
    {
        return distanceType;
    }
}
