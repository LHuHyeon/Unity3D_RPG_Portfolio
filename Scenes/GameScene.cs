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
        GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "TestPlayer2");
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(_player);

        Invoke("DelayScene", 3f);
    }

    void DelayScene()
    {
        Managers.Game.Init();
        Managers.Game._playScene = Managers.UI.ShowSceneUI<UI_PlayScene>();
        for(int i=0; i<5; i++)
        {
            GameObject obj = Managers.Game.Spawn(Define.WorldObject.Monster, "Monster/Skeleton1");
            obj.transform.position += new Vector3(0, 0, 5f);
        }
    }

    // 나중엔 로그인 시 진행
    void OnDataRequest()
    {
        StartCoroutine(Managers.Data.DataRequest(Define.StartNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.LevelNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.SkillNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.UseItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.WeaponItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.ArmorItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.DropItemNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.MonsterNumber));
    }

    public override void Clear()
    {

    }
}
