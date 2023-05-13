using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    Coroutine co;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;  // 타입 설정

        OnDataRequest();

        gameObject.GetOrAddComponent<CursorController>();   // 마우스 커서 생성

        Invoke("DelayScene", 3f);

        // GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        // Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(_player);
    }

    void DelayScene()
    {
        Managers.Game.Init();
        // Managers.UI.ShowSceneUI<UI_PlayScene>();
    }

    // 나중엔 로그인 시 진행
    void OnDataRequest()
    {
        StartCoroutine(Managers.Data.DataRequest(Define.StartNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.LevelNumber));
    }

    public override void Clear()
    {

    }
}
