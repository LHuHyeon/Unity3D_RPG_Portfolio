using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
[ 퀘스트 알림 Slot 스크립트 ]
1. PlayScene에서 우측 가운데에 진행 중인 퀘스트를 띄울 때 사용한다.
*/

public class UI_QuestNoticeSlot : UI_Base
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescText;

    public QuestData _quest;

    string targetName;

    void Update()
    {
        if (_quest.IsNull() == true)
            return;

        string questText = targetName + " : " + _quest.currnetTargetCount + " / " + _quest.targetCount;
        questDescText.text = questText;
    }

    public void SetInfo(QuestData quest)
    {
        _quest = quest;

        questNameText.text = quest.titleName;
        targetName = Managers.Data.Monster[_quest.targetId].GetComponent<MonsterStat>().Name;
        
        string questText = targetName + " : " + _quest.currnetTargetCount + " / " + _quest.targetCount;
        questDescText.text = questText;
    }
}
