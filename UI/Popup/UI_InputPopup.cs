using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/*
 * File :   UI_InputPopup.cs
 * Desc :   입력이 필요할 때 Popup UI (닉네임 입력 등..)
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : SetInfo()             - 새 정보 설정 ( 확인 클릭 시 Invoke 호출할 Action 받기 )
 &
 &  [Private]
 &  : OnClickYesButton()    - 확인 버튼 클릭 시 호출
 &  : OnClickNoButton()     - 취소 버튼 클릭 시 호출
 *
 */

public class UI_InputPopup : UI_Popup
{
    enum Buttons
    {
        NoButton,
        YesButton,
    }

    [SerializeField]
    private TMP_InputField  _inputField;

    [SerializeField]
    private TextMeshProUGUI _messageText;

    private string          _regex;         // 정규식

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        // 버튼 기능 등록
        GetButton((int)Buttons.YesButton).onClick.AddListener(OnClickYesButton);
        GetButton((int)Buttons.NoButton).onClick.AddListener(OnClickNoButton);

        return true;
    }

    // 기능 설정
    Action<string> _onClickYesButton;
    Action _onClickNoButton;
    public void SetInfo(Action<string> onClickYesButton, string messageText, string placeholderText, string regex, Action onClickNoButton=null)
    {
        _onClickYesButton = onClickYesButton;
        _onClickNoButton = onClickNoButton;
        _messageText.text = messageText;
        _regex = regex;

        _inputField.placeholder.GetComponent<TextMeshProUGUI>().text = placeholderText;
        _inputField.Select();
    }

    private void OnClickYesButton()
    {
        Regex regex = new Regex(_regex);
        if (regex.IsMatch(_inputField.text))
        {
            Managers.UI.ClosePopupUI(this);

            // 확인 기능 실행
            if (_onClickYesButton.IsNull() == false)
                _onClickYesButton.Invoke(_inputField.text);
        }
        else
        {
            // 경고문 생성
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("한글|영어|숫자 2글자 이상 8글자 이하", Color.red);
        }
    }

    private void OnClickNoButton()
    {
        Managers.UI.ClosePopupUI(this);

        // 취소 기능 실행
        if (_onClickNoButton.IsNull() == false)
            _onClickNoButton.Invoke();
    }
}
