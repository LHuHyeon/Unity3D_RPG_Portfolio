using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ConfirmPopup : UI_Popup
{
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

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        GetText((int)Texts.MessageText).text = _text;

        return true;
    }
    
    Action _onClickYesButton;
    public void SetInfo(Action onClickYesButton, string text)
    {
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
