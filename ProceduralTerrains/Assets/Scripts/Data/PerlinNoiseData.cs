using UnityEngine;

[CreateAssetMenu]
public class PerlinNoiseData : NoiseData
{
    public float noiseScale;

    public int numberOctaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public override NoiseType GetNoiseType()
    {
        return NoiseType.PerlinNoise;
    }

    private void OnValidate()
    {
        lacunarity = Mathf.Max(lacunarity, 1);
        numberOctaves = Mathf.Max(numberOctaves, 1);
        noiseScale = Mathf.Max(0.0001f, noiseScale);
    }

    public override float GetNoiseScale()
    {
        return noiseScale;
    }

    public override int GetNumberOctaves()
    {
        return numberOctaves;
    }

    public override float GetPersistance()
    {
        return persistance;
    }

    public override float GetLacunarity()
    {
        return lacunarity;
    }

    public override int GetSeed()
    {
        return seed;
    }

    public override Vector2 GetOffset()
    {
        return offset;
    }
}
