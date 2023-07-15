using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkinnedData
{
    public string sharedMeshName;
    public Bounds bounds;
    public List<string> bones;
    public string rootBoneName;
}
