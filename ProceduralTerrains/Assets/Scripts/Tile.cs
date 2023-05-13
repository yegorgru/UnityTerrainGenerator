using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    protected GameObject meshObject;
    public void Remove()
    {
        meshObject.transform.SetParent(null);
        Object.DestroyImmediate(meshObject);
    }
}
