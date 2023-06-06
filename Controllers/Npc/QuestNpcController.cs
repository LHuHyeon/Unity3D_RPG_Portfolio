using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNpcController : NpcController
{
    public Define.QuestType questType = Define.QuestType.Unknown;

    public int questId;
    public int talkId;

    TalkData talkData;
    QuestData questData;

    public override void Init()
    {
        base.Init();

        Invoke("DelayData", 3f);
    }

    // TODO : 나중에 Init으로 옮기기
    void DelayData()
    {
        talkData = Managers.Data.Talk[talkId];
        questData = Managers.Data.Quest[questId];
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
        if (talkData == null)
            return;

        Managers.Game.isTalk = true;

        // 이미 퀘스트를 클리어 했는가?
        if (questData.isClear == true)
        {
            Talk(talkData.basicsTalk);
            return;
        }

        // 퀘스트가 수락 중이라면
        if (questData.isAccept == true)
        {
            Talk(talkData.procTalk);
            return;
        }
        
        // 레벨이 되면 퀘스트 대화 시작
        if (questData.minLevel <= Managers.Game.Level)
        {
            Talk(talkData);
            return;
        }

        // 여기까지 오면 퀘스트 해당 없으므로 기본 대화 진행
        Talk(talkData.basicsTalk);
    }

    // 대화 시작
    void Talk(string text)
    {
        UI_TalkPopup talkPopup = Managers.Game._playScene._talk;
        Managers.UI.OnPopupUI(talkPopup);
        talkPopup.SetInfo(text);
    }

    void Talk(TalkData texts)
    {
        UI_TalkPopup talkPopup = Managers.Game._playScene._talk;
        Managers.UI.OnPopupUI(talkPopup);
        talkPopup.SetInfo(texts, questData);
    }
}
