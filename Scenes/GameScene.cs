using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    [SerializeField]
    Transform playerSpawn;

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Game;  // 타입 설정

        if (Managers.Data.isData == false)
            OnDataRequest();

        if (Managers.Game.GetPlayer() == false)
        {
            gameObject.GetOrAddComponent<CursorController>();   // 마우스 커서 생성
            GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "TestPlayer2");
            _player.transform.position = playerSpawn.position;
            DontDestroyOnLoad(_player);
        }

        if (Managers.Game._playScene == false)
            Invoke("DelayScene", 3f);

        if (Managers.Game.beforeSpawnPos != Vector3.zero)
            Managers.Game.GetPlayer().transform.position = Managers.Game.beforeSpawnPos;

        Managers.Game.StopPlayer();
        
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(Managers.Game.GetPlayer());
    }

    void DelayScene()
    {
        Managers.Game.Init();
        Managers.Game._playScene = Managers.UI.ShowSceneUI<UI_PlayScene>();
        DontDestroyOnLoad(Managers.Game._playScene.gameObject);
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
        StartCoroutine(Managers.Data.DataRequest(Define.ShopNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.TalkNumber));
        StartCoroutine(Managers.Data.DataRequest(Define.QuestNumber));

        Managers.Data.isData = true;
    }

    public override void Clear()
    {

    }
}
