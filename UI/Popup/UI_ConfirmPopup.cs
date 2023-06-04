using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    enum Texts
    {
        MessageText,
    }

    string _text;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        GetText((int)Texts.MessageText).text = _text;

        return true;
    }
    
    Action _onClickYesButton;
    public void SetInfo(Action onClickYesButton, string text)
    {
        Managers.UI.SetOrder(GetComponent<Canvas>());
        
        _onClickYesButton = onClickYesButton;
        _text = text;
    }

    void OnClickYesButton()
    {
        Managers.UI.ClosePopupUI(this);
        if (_onClickYesButton != null)
            _onClickYesButton.Invoke();
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
