using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/*
[ 입력 Popup 스크립트 ]
1. 입력이 필요한 상황에 띄울 수 있는 Popup이다.
2. 자주 호출되는 함수 : SetInfo()
3. 현재는 닉네임 입력 전용으로 사용하고 있다.
*/

public class UI_InputPopup : UI_Popup
{
    enum Buttons
    {
        NoButton,
        YesButton,
    }

    [SerializeField]
    TMP_InputField _inputField;

    [SerializeField]
    TextMeshProUGUI _messageText;

    string _regex;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }

    Action<string> _onClickYesButton;
    public void SetInfo(Action<string> onClickYesButton, string messageText, string placeholderText, string regex)
    {
        // TODO : 지금은 닉네임 전용으로 사용하지만 또 다른 Input이 필요하면 텍스트 클래스 만들어서 관리
        _onClickYesButton = onClickYesButton;
        _messageText.text = messageText;
        _regex = regex;

        _inputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeholderText;
        _inputField.Select();
    }

    void OnClickYesButton()
    {
        Regex regex = new Regex(_regex);
        if (regex.IsMatch(_inputField.text))
        {
            Managers.UI.ClosePopupUI(this);
            if (_onClickYesButton.IsNull() == false)
                _onClickYesButton.Invoke(_inputField.text);
        }
        else
        {
            Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo("한글|영어|숫자 2글자 이상 8글자 이하", Color.red);
        }
    }

    void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
