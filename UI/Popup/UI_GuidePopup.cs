using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 가이드 Popup 스크립트 ]
1. Level Up, 돈 부족 등.. 상황에 띄울 수 있는 Text Popup이다.
2. 자주 호출되는 함수 : SetInfo()
*/

public class UI_GuidePopup : UI_Popup
{
    enum Texts
    {
        MessageText,
    }

    Color _color;
    string _messageText;

    Coroutine co;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));

        GetText((int)Texts.MessageText).color = _color;
        GetText((int)Texts.MessageText).text = _messageText;

        return true;
    }

    public void SetInfo(string messageText, Color color)
    {
        _color = color;
        _messageText = messageText;

        if (co.IsNull() == false) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

    IEnumerator MessageCoroutine()
    {
        if (GetText((int)Texts.MessageText).IsNull() == false)
            GetText((int)Texts.MessageText).transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(1f);

        // 점점 사라지며 올라가기
        for(float i=1.0f; i>=0.0f; i-=0.01f)
        {
            _color.a = i;
            GetText((int)Texts.MessageText).color = _color;

            GetText((int)Texts.MessageText).transform.localPosition += Vector3.up * 0.7f;

            yield return null;
        }
        Managers.UI.ClosePopupUI(this);
    }
}
