using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * File :   UI_QuestNotice.cs
 * Desc :   퀘스트 NPC에 !, ?를 띄운다.
 *
 & Functions
 &  SetInfo()   - 기능 설정 ( 알람 위치 )
 *
 */

public class UI_QuestNotice : UI_Base
{
    [SerializeField]
    private TextMeshProUGUI     _noticeText;    // 알림 text

    public UI_QuestNotice SetInfo(string noticeText, Vector3 pos = new Vector3())
    {
        _noticeText.text = noticeText;

        if (pos != Vector3.zero)
            transform.position = pos + (Vector3.up * 3f);

        return this;
    }
}
