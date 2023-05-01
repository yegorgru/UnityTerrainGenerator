using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TerrainData : ScriptableObject
{
    public float uniformScale = 5f;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
}
