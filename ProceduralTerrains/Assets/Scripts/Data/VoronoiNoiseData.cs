using UnityEngine;

[CreateAssetMenu]
public class VoronoiNoiseData : NoiseData
{
    public int seed;
    public int pivotNumber;
    [Range(0f, 1f)]
    public float persistance;
    public int numberOctaves;

    private void OnValidate()
    {
        pivotNumber = Mathf.Max(1, pivotNumber);
        numberOctaves = Mathf.Max(numberOctaves, 1);
    }

    public override NoiseType GetNoiseType()
    {
        return NoiseType.VoronoiNoise;
    }

    public override int GetSeed()
    {
        return seed;
    }

    public override int GetPivotNumber()
    {
        return pivotNumber;
    }

    public override int GetNumberOctaves()
    {
        return numberOctaves;
    }

    public override float GetPersistance()
    {
        return persistance;
    }
}
