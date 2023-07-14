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

        // 플레이어 캐릭터 생성
        if (Managers.Game.GetPlayer() == false)
        {
            GameObject _player = Managers.Game.Spawn(Define.WorldObject.Player, "Player");
            _player.transform.position = playerSpawn.position;
            DontDestroyOnLoad(_player);
        }
        else
            Managers.Game.StopPlayer();

        // UI 생성
        if (Managers.Game._playScene == false)
        {
            Managers.Game.Init();
            Managers.Game._playScene = Managers.UI.ShowSceneUI<UI_PlayScene>();
            DontDestroyOnLoad(Managers.Game._playScene.gameObject);
        }

        // 플레이어 세이브 위치 이동
        if (Managers.Game.CurrentPos != Vector3.zero)
            Managers.Game.GetPlayer().transform.position = Managers.Game.CurrentPos;

        // 클릭 Effect 생성
        if (Managers.Game.GetPlayer() != null)
        {
            GameObject clickMoveEffect = Managers.Resource.Instantiate("Effect/ClickMoveEffect");
            clickMoveEffect.SetActive(false);

            Managers.Game.GetPlayer().GetComponent<PlayerController>().clickMoveEffect = clickMoveEffect;
        }
        
        // 카메라 조정
        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(Managers.Game.GetPlayer());
    }

    public override void Clear()
    {

    }
}
