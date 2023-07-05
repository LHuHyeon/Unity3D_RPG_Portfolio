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

        if (Managers.Game.GetPlayer() == false)
        {
            gameObject.GetOrAddComponent<CursorController>();   // 마우스 커서 생성
            GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "TestPlayer2");
            _player.transform.position = playerSpawn.position;
            DontDestroyOnLoad(_player);
        }
        else
            Managers.Game.StopPlayer();

        if (Managers.Game._playScene == false)
        {
            Managers.Game.Init();
            Managers.Game._playScene = Managers.UI.ShowSceneUI<UI_PlayScene>();
            DontDestroyOnLoad(Managers.Game._playScene.gameObject);
        }

        if (Managers.Game.beforeSpawnPos != Vector3.zero)
            Managers.Game.GetPlayer().transform.position = Managers.Game.beforeSpawnPos;
        
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(Managers.Game.GetPlayer());
    }

    public override void Clear()
    {

    }
}
