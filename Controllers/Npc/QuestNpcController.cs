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

        // 현재 클리어한 퀘스트가 있는지
        while (true)
        {
            for(int i=0; i<Managers.Game.ClearQuest.Count; i++)
            {
                if (questDataList[nextQuest].id == Managers.Game.ClearQuest[i].id)
                {
                    Debug.Log("NextQuest()");
                    NextQuest();
                    continue;
                }
            }

            break;
        }

        // 현재 진행 중인 퀘스트의 id와 같다면 퀘스트 진행사항 넣어주기
        for(int i=0; i<Managers.Game.CurrentQuest.Count; i++)
        {
            if (questDataList[nextQuest].id == Managers.Game.CurrentQuest[i].id)
            {
                Debug.Log("QuestAccept()");
                currentQuest = questDataList[nextQuest] = Managers.Game.CurrentQuest[i];
            }
        }
    }

    public override void Interact()
    {
        if (Managers.Game.isPopups[Define.Popup.Talk] == true)
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

        Managers.Game.isPopups[Define.Popup.Talk] = true;

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

                // 알람 되어 있는 퀘스트라면 삭제
                Managers.Game._playScene._quest.CloseQuestNotice(currentQuest);

                // 다음 퀘스트 확인
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
        nextQuest++;

        // 퀘스트 개수 확인
        if (nextQuest == questDataList.Count)
            return;

        currentQuest = questDataList[nextQuest];
        currentTalk = talkDataList[nextQuest];
    }
}
