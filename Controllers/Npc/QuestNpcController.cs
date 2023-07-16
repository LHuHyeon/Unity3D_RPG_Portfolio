using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 퀘스트 NPC 컨트롤러 스크립트 ]
1. 대화 종류 : 일반 대화, 퀘스트 시작 대화, 수락 대화, 거절 대화, 퀘스트 진행 중 대화, 클리어 대화
2. 퀘스트를 id로 미리 저장하고 Init에서 id에 맞는 퀘스트 데이터를 받아온다.
3. 클리어 상태, 수락 상태, 미수락 상태 등 상황에 맞게 구현.
*/

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
                    NextQuest();
                    continue;
                }
            }

            break;
        }

        Invoke("DelayInit", 0.1f);
    }

    void DelayInit()
    {
        // 퀘스트 중이 아니라면 ! 띄우기
        if (currentQuest.isAccept == false)
        {
            if (currentQuest.minLevel <= Managers.Game.Level)
                Managers.Game._playScene._quest.noticeObject.SetInfo("!", transform.position);
        }
        else
            Managers.Game._playScene._quest.noticeObject.SetInfo("", transform.position);
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

                // 클리어 퀘스트에 등록
                Managers.Game.RefreshQuest();

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
        {
            Managers.Game._playScene._quest.noticeObject.SetInfo("", transform.position);
            return;
        }

        currentQuest = questDataList[nextQuest];
        currentTalk = talkDataList[nextQuest];

        // 레벨 체크
        if (currentQuest.minLevel <= Managers.Game.Level)
            Managers.Game._playScene._quest.noticeObject.SetInfo("!", transform.position);
    }
}
