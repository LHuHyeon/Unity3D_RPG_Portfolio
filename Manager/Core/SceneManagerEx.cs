using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// SceneManager은 이름 충돌 때문에 Ex를 붙임.
public class SceneManagerEx
{
    // 다른 스크립트에서 읽을 변수
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    // 매핑을 한다고 해서 부하의 걱정을 할 수 있지만
    // Scene 같은 경우 자주 사용하지 않기 때문에 너무 걱정할 필요는 없다.
    public void LoadScene(Define.Scene type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        CurrentScene.Clear();
    }
}
