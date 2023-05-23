using UnityEngine;

[CreateAssetMenu]
public class ValueNoiseData : NoiseData
{
    public float noiseScale;
    public Vector2 offset;
    public int gridFrequency;

    private void OnValidate()
    {
        noiseScale = Mathf.Max(0.0001f, noiseScale);
        gridFrequency = Mathf.Max(1, gridFrequency);
    }

    public override NoiseType GetNoiseType()
    {
        return NoiseType.ValueNoise;
    }

    public override float GetNoiseScale()
    {
        return noiseScale;
    }

    public override Vector2 GetOffset()
    {
        return offset;
    }

    public override int GetGridFrequency()
    {
        return gridFrequency;
    }
}
