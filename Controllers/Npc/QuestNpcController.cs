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

    bool isQuest;

    List<QuestData> questDataList;
    List<TalkData> talkDataList;

    QuestData currentQuest;
    TalkData currentTalk;

    UI_QuestNotice noticeObject;

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
        isQuest = true;

        Invoke("DelayInit", 0.0001f);
    }

    void DelayInit()
    {
        // 현재 클리어한 퀘스트가 있는지
        for(int i=0; i<Managers.Game.ClearQuest.Count; i++)
        {
            if (questDataList[nextQuest].id == Managers.Game.ClearQuest[i].id)
            {
                NextQuest();
                continue;
            }
        }

        // 수락 확인
        for(int i=0; i<Managers.Game.CurrentQuest.Count; i++)
        {
            if (currentQuest.id == Managers.Game.CurrentQuest[i].id)
                currentQuest = Managers.Game.CurrentQuest[i];
        }
        
        noticeObject = Managers.UI.MakeWorldSpaceUI<UI_QuestNotice>();
    }

    void FixedUpdate()
    {
        if (noticeObject.IsNull() == true)
            return;

        // 실시간 npc 위의 퀘스트 상태 표시
        if (currentQuest.minLevel > Managers.Game.Level)
            return;

        if (currentQuest.isClear == true || isQuest == false)
        {
            noticeObject.SetInfo("", transform.position);
            return;
        }

        if (currentQuest.currnetTargetCount >= currentQuest.targetCount)
        {
            noticeObject.SetInfo("?");
            return;
        }
        
        if (currentQuest.isAccept == true)
            noticeObject.SetInfo("", transform.position);
        else
            noticeObject.SetInfo("!", transform.position);
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
        if (currentTalk.IsNull() == true)
            return;

        // 이미 퀘스트를 클리어 했는가? or 퀘스트가 없거나
        if (currentQuest.isClear == true || isQuest == false)
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
                // 인벤 크기 체크
                if (Managers.Game._playScene._inventory.IsInvenMaxSize() == true)
                {
                    Managers.UI.MakeSubItem<UI_Guide>().SetInfo("인벤토리가 가득 찼습니다.", Color.red);
                    return;
                }

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
            isQuest = false;
            return;
        }

        currentQuest = questDataList[nextQuest];
        currentTalk = talkDataList[nextQuest];

        isQuest = true;
    }
}
