using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * File :   UI_QuestNoticeSlot.cs
 * Desc :   Scene UI에 생성된 퀘스트 알람 기능
 *
 & Functions
 & : FixedUpdate()  - 실시간 퀘스트 내용 업데이트
 & : SetInfo()      - 기능 설정 (퀘스트 정보 받기)
 *
 */

public class UI_QuestNoticeSlot : UI_Base
{
    enum Texts
    {
        QuestNameText,
        QuestDescText
    }

    public QuestData    _quest;

    private string      _targetName;        // 목표 이름
    private string      _questNameText;     // 퀘스트 제목
    private string      _qeustDescText;     // 퀘스트 내용

    private bool        isSuccess = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));

        GetText((int)Texts.QuestNameText).text = _questNameText;
        GetText((int)Texts.QuestDescText).text = _qeustDescText;

        return true;
    }

    void FixedUpdate()
    {
        if (_quest.IsNull() == true || isSuccess == true)
            return;

        // 퀘스트 목표 달성 시
        if (_quest.currnetTargetCount == _quest.targetCount)
        {
            // text 완료 표시
            GetText((int)Texts.QuestNameText).text = _quest.titleName + $@"<color=yellow> [완료]</color>";
            isSuccess = true;
        }

        // 퀘스트 진행 상황 표시
        if (GetText((int)Texts.QuestDescText).IsNull() == false)
            GetText((int)Texts.QuestDescText).text = $"{_targetName} : {_quest.currnetTargetCount} / {_quest.targetCount}";
    }

    public void SetInfo(QuestData quest)
    {
        _quest = quest;

        // 퀘스트 타겟 이름
        _targetName = Managers.Data.Monster[_quest.targetId].GetComponent<MonsterStat>().Name;

        // 퀘스트 제목
        _questNameText = quest.titleName;
        _qeustDescText = $"{_targetName} : {_quest.currnetTargetCount} / {_quest.targetCount}";
    }
}
