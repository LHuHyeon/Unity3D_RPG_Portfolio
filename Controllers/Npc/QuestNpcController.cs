using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpcController : NpcController
{
    public Define.QuestType questType = Define.QuestType.Unknown;

    public int[] questId;
    public int[] talkId;

    int nextQuest = 0;

    List<QuestData> questDataList;
    List<TalkData> talkDataList;

    QuestData currentQuest;
    TalkData currentTalk;

    public override void Init()
    {
        base.Init();

        questDataList = new List<QuestData>();
        talkDataList = new List<TalkData>();

        for(int i=0; i<questId.Length; i++)
        {
            questDataList.Add(Managers.Data.Quest[questId[i]]);
            talkDataList.Add(Managers.Data.Talk[talkId[i]]);
        }

        nextQuest = 0;

        currentQuest = questDataList[nextQuest];
        currentTalk = talkDataList[nextQuest];
    }

    public override void Interact()
    {
        if (Managers.Game.isTalk == true)
            return;

        if (Managers.Game.IsInteract)
        {
            Managers.UI.CloseAllPopupUI();
            Managers.Game.StopPlayer();
            TalkCheck();
        }
    }

    void TalkCheck()
    {
        if (currentTalk == null)
            return;

        Managers.Game.isTalk = true;

        // 이미 퀘스트를 클리어 했는가?
        if (currentQuest.isClear == true)
        {
            Talk(currentTalk.basicsTalk);
            return;
        }

        // 퀘스트가 수락 중이라면
        if (currentQuest.isAccept == true)
        {
            // 퀘스트 목표 개수 충족 되면
            if (currentQuest.currnetTargetCount >= currentQuest.targetCount)
            {
                Talk(currentTalk.clearTalk);
                currentQuest.QuestClear();

                NextQuest();
            }
            else
                Talk(currentTalk.procTalk);
                
            return;
        }
        
        // 레벨이 되면 퀘스트 대화 시작
        if (currentQuest.minLevel <= Managers.Game.Level)
        {
            Talk(currentTalk);
            return;
        }

        // 여기까지 오면 퀘스트 해당 없으므로 기본 대화 진행
        Talk(currentTalk.basicsTalk);
    }

    // 대화 시작
    void Talk(string text)
    {
        UI_TalkPopup talkPopup = Managers.Game._playScene._talk;
        Managers.UI.OnPopupUI(talkPopup);
        talkPopup.SetInfo(text, npcName);
    }

    void Talk(TalkData texts)
    {
        UI_TalkPopup talkPopup = Managers.Game._playScene._talk;
        Managers.UI.OnPopupUI(talkPopup);
        talkPopup.SetInfo(texts, currentQuest, npcName);
    }

    void NextQuest()
    {
        // 알람 되어 있는 퀘스트라면 삭제
        Managers.Game._playScene._quest.CloseQuestNotice(currentQuest);

        nextQuest++;

        // 퀘스트 개수 확인
        if (nextQuest == questDataList.Count)
            return;

        currentQuest = questDataList[nextQuest];
        currentTalk = talkDataList[nextQuest];
    }
}
