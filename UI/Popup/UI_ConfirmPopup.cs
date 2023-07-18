using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 확인 Popup 스크립트 ]
1. 확인이 필요한 상황에 띄울 수 있는 Popup이다.
2. 자주 호출되는 함수 : SetInfo()
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

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }
    
    Action _onClickYesButton;
    public void SetInfo(Action onClickYesButton, string text)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        
        _onClickYesButton = onClickYesButton;
        _Messagetext.text = text;
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);
        if (_onClickYesButton.IsNull() == false)
            _onClickYesButton.Invoke();
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
