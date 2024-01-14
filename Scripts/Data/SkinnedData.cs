using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   SkinnedData.cs
 * Desc :   커스텀이나 세이브를 불러올 때 사용될 데이터
 *          [SkinnedMeshRenderer 교체 방법]: https://lhuhyeon.github.io/posts/Unity-SkinnedMeshRenderer-Change/
 */

[Serializable]
public class SkinnedData
{
    public string sharedMeshName;
    public Bounds bounds;
    public List<string> bones;
    public string rootBoneName;
}
