using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 기본 씬 스크립트 ]
1. 모든 씬 스크립트는 BaseScene 클래스를 상속받는다.
2. BaseScene은 씬이 실행될 때 기본적으로 필요한 기능들을 생성or호출한다.
*/

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));

        if (obj.IsNull() == true)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
