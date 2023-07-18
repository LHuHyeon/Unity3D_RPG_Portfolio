using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
