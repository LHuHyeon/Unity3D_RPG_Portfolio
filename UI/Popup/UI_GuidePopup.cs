using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(MessageCoroutine());
    }

    IEnumerator MessageCoroutine()
    {
        if (GetText((int)Texts.MessageText) != null)
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
