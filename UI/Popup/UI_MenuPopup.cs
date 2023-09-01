using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_MenuPopup.cs
 * Desc :   Esc Menu Popup UI
 *          현재 띄어진 Popup이 없다면 Menu Popup을 활성화한다.
 *
 & Functions
 &  [Public]
 &  : Init() - 초기 설정
 &
 &  [Private]
 &  : OnMenuPopup()             - 메뉴 활성화or비활성화
 &  : OnMenu()                  - 메뉴 활성화 진행
 &  : OnClickContinueButton()   - 진행 버튼
 &  : OnClickSaveButton()       - 세이브 버튼
 &  : OnClickAppExitButton()    - 나가기 버튼
 &  : Exit()                    - 초기화
 *
 */

public class UI_MenuPopup : UI_Popup
{
    enum Buttons
    {
        ContinueButton,
        SaveButton,
        AppExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        popupType = Define.Popup.Menu;

        // 자식 객체 불러오기
        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.ContinueButton).onClick.AddListener(OnClickContinueButton);
        GetButton((int)Buttons.SaveButton).onClick.AddListener(OnClickSaveButton);
        GetButton((int)Buttons.AppExitButton).onClick.AddListener(OnClickAppExitButton);

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnMenuPopup;
        Managers.Input.KeyAction += OnMenuPopup;

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    // 메뉴 활성화
    private void OnMenuPopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.Game.isPopups[Define.Popup.Menu] = !Managers.Game.isPopups[Define.Popup.Menu];

            // 메뉴 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Menu])
                OnMenu();
            else
                Exit();
        }
    }

    private void OnMenu()
    {
        // 현재 활성화 중인 Popup이 없다면
        if (Managers.UI.ClosePopupUI() == false)
        {
            // 메뉴 활성화
            Managers.UI.OnPopupUI(this);
            Time.timeScale = 0;
        }
        else
        {
            // 메뉴 끄기
            Managers.Game.isPopups[Define.Popup.Menu] = false;
            Managers.Game._playScene._slotTip.OnSlotTip(false);
        }
    }
    
    // 게임 진행 버튼
    private void OnClickContinueButton()
    {
        Exit();
    }

    // 게임 세이브 버튼
    private void OnClickSaveButton()
    {
        Managers.Game.SaveGame();
        Exit();
    }

    // 게임 나가기 버튼
    private void OnClickAppExitButton()
    {
        Exit();
        Application.Quit();
    }

    // 초기화
    private void Exit()
    {
        Time.timeScale = 1;
        Managers.UI.ClosePopupUI(this);
    }
}
