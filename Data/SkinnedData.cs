using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ SkinnedMeshRenderer 데이터 ]
1. 커스텀이나 세이브를 불러올 때 사용될 저장 데이터
2. Mesh, Bones는 string으로 받아 이름으로 찾아 사용됨.
*/

[Serializable]
public class SkinnedData
{
    public string sharedMeshName;
    public Bounds bounds;
    public List<string> bones;
    public string rootBoneName;
}
