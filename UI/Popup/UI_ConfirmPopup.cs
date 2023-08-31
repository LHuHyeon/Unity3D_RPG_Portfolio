using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   UI_ConfirmPopup.cs
 * Desc :   확인/취소 Popup
 *
 & Functions
 &  [Public]
 &  : Init()    - 초기 설정
 &  : SetInfo() - 새 정보 설정 ( 확인 클릭 시 Invoke 호출할 Action 받기 )
 &
 &  [Private]
 &  : OnClickYesButton()    - 확인 클릭 시 호출
 &  : OnClickNoButton()     - 취소 클릭 시 호출
 *
 */

public class UI_ConfirmPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
    }

    enum Buttons
    {
        YesButton,
        NoButton,
    }

    [SerializeField]
    TextMeshProUGUI _Messagetext;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // 버튼 Event 설정
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }
    
    // 새 정보 설정 ( Action 받기 )
    Action _onClickYesButton;
    public void SetInfo(Action onClickYesButton, string text)
    {
        // Order + 1
        Managers.UI.SetOrder(GetComponent<Canvas>());
        
        _onClickYesButton = onClickYesButton;
        _Messagetext.text = text;
    }

    // 확인 버튼
    private void OnClickYesButton()
    {
        // Action Invoke 실행
        Managers.UI.ClosePopupUI(this);
        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke();
    }

    // 취소 버튼
    private void OnClickNoButton()
    {
        // Popup 비활성화
        Managers.UI.ClosePopupUI(this);
    }
}
