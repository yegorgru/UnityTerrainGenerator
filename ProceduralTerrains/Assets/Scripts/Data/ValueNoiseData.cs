using UnityEngine;

[CreateAssetMenu]
public class ValueNoiseData : NoiseData
{
    public Vector2 offset;
    public int gridFrequency;
    public ValueInterpolationType valueInterpolationType;

    private void OnValidate()
    {
        gridFrequency = Mathf.Max(1, gridFrequency);
    }

    public override NoiseType GetNoiseType()
    {
        return NoiseType.ValueNoise;
    }

    public override Vector2 GetOffset()
    {
        return offset;
    }

    public override int GetGridFrequency()
    {
        return gridFrequency;
    }

    public override ValueInterpolationType GetValueInterpolationType()
    {
        return valueInterpolationType;
    }
}
