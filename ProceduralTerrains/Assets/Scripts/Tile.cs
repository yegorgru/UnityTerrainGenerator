using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject meshObject;
    public void Remove()
    {
        meshObject.transform.SetParent(null);
        Object.DestroyImmediate(meshObject);
    }
}
