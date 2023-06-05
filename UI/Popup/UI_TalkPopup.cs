using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TalkPopup : UI_Popup
{
    /*
    1. 대화 진행 (이름, 내용)
    2. 퀘스트 띄우기
    3. 다음 버튼
    */

    enum Gameobejcts
    {
        TalkBackground,
        QuestJournal,
        QuestRewardGrid,
    }

    enum Buttons
    {
        NextButton,
    }

    enum Texts
    {
        NameText,
        TalkText,
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        return true;
    }

    // 퀘스트 진행하는 NPC, 퀘스트 ID
    public void SetInfo()
    {

    }

    // 퀘스트 보여주기
    public void OnQuest()
    {

    }

    public void Clear()
    {
        
    }
}
