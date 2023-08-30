using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   BaseScene.cs
 * Desc :   모든 Scene은 BaseScene을 상속받는다.
 *          BaseScene은 Scene이 로드될 때 기본적인 기능을 수행
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
        
        Camera.main.gameObject.GetOrAddComponent<CursorController>();
    }

    public abstract void Clear();
}
