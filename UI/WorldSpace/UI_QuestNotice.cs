using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 퀘스트 NPC에 !, ?를 띄운다.
public class UI_QuestNotice : UI_Base
{
    [SerializeField]
    TextMeshProUGUI _noticeText;

    public UI_QuestNotice SetInfo(string noticeText, Vector3 pos = new Vector3())
    {
        _noticeText.text = noticeText;

        if (pos != Vector3.zero)
            transform.position = pos + (Vector3.up * 3f);

        return this;
    }
}
