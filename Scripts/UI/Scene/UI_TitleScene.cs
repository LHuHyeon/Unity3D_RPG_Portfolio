using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * File :   UI_TitleScene.cs
 * Desc :   게임 플레이 Scene UI
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &
 &  [Private]
 &  : OnClickStartButton()  - 게임 시작 버튼
 &  : OnClickLoadButton()   - 세이브 로드 버튼
 &  : OnClickExitButton()   - 게임 나가기 버튼
 *
 */

public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        StartButton,
        LoadButton,
        ExitButton,
    }

    enum Texts
    {
        LoadButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // 버튼 기능 등록
        GetButton((int)Buttons.StartButton).onClick.AddListener(OnClickStartButton);
        GetButton((int)Buttons.LoadButton).onClick.AddListener(OnClickLoadButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(OnClickExitButton);

        // 세이브 로드 여부 확인
        if (Managers.Game.IsSaveLoad() == false)
        {
            Color _color = GetText((int)Texts.LoadButtonText).color;
            _color.a = 0.5f;
            GetText((int)Texts.LoadButtonText).color = _color;

            string path = "Art/UI/Classic_RPG_GUI/Parts/mid_button_off";
            GetButton((int)Buttons.LoadButton).GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>(path);
        }

        return true;
    }
    
    // 시작 버튼
    private void OnClickStartButton()
    {
        Managers.Scene.LoadScene(Define.Scene.PlayerCustom);
    }

    // 세이브 로드 버튼
    private void OnClickLoadButton()
    {
        if (Managers.Game.LoadGame() == false)
            return;
            
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷 연결이 안되었을 때 행동
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("네트워크 연결이 필요합니다.", Color.red);
        }
        else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // 데이터로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 6);
        }
        else
        {
            // 와이파이로 연결이 되었을 때 행동
            Managers.UI.ShowPopupUI<UI_LoadPopup>().SetInfo(Define.Scene.Game, 7);
        }
    }

    // 나가기 버튼
    private void OnClickExitButton()
    {
        Application.Quit();
    }
}
