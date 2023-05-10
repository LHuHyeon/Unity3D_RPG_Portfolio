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

        StartCoroutine(Managers.Google.DataRequest());

        gameObject.GetOrAddComponent<CursorController>();   // 마우스 커서 생성

        // GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        // Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(_player);
    }

    public override void Clear()
    {

    }
}
