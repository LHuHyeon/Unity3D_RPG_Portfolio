using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestPopup : UI_Popup
{
    enum Gameobejcts
    {
        Content,
        QuestJournal,
        QuestRewardGrid,
    }

    enum Buttons
    {
        ExitButton,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
        QuestNoticeCountText,
    }

    // 퀘스트 알림 개수
    int maxquestNoticeCount = 5;
    public List<UI_QuestNoticeSlot> questNoticeList;

    QuestData currentClickQuest;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        questNoticeList = new List<UI_QuestNoticeSlot>();

        Managers.Input.KeyAction -= OnQuestPopup;
        Managers.Input.KeyAction += OnQuestPopup;
        
        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        // 퀘스트 목표 계속 새로고침
        if (Managers.Game.isQuest == true && currentClickQuest != null)
        {
            string str = currentClickQuest.targetDescription + "\n" + currentClickQuest.currnetTargetCount + " / " + currentClickQuest.targetCount;
            GetText((int)Texts.QuestTargetText).text = str;
        }
    }

    void OnQuestPopup()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Managers.Game.isQuest = !Managers.Game.isQuest;

            if (Managers.Game.isQuest)
            {
                Managers.UI.OnPopupUI(this);
                RefreshUI();
            }
            else
                Managers.UI.ClosePopupUI(this);
        }
    }

    void SetInfo()
    {
        GetButton((int)Buttons.ExitButton).onClick.AddListener(()=>{Managers.UI.ClosePopupUI(this); Managers.Game.isQuest = false;});

        // 미리보기 삭제
        foreach(Transform child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(child.gameObject);

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);
    }

    // 퀘스트 목록을 누르면
    public void OnQuest(QuestData quest)
    {
        if (quest == null)
        {
            Debug.Log("OnQuest() : quest Null");
            return;
        }

        currentClickQuest = quest;

        GetText((int)Texts.QuestTitleText).text = quest.titleName;
        GetText((int)Texts.QuestDescText).text = quest.description;
        GetText((int)Texts.QuestTargetText).text = quest.targetDescription;
        GetText((int)Texts.QuestRewardGoldText).text = quest.rewardGold.ToString();
        GetText((int)Texts.QuestRewardExpText).text = quest.rewardExp.ToString();

        foreach(Transform child in GetObject((int)Gameobejcts.QuestRewardGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<quest.rewardItems.Count; i++)
        {
            UI_RewardItem rewardItem = Managers.UI.MakeSubItem<UI_RewardItem>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo(Managers.Data.Item[quest.rewardItems[i].ItemId], quest.rewardItems[i].itemCount);
        }

        GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
    }

    // 씬에 퀘스트 알림 추가
    public bool SetQuestNotice(QuestData quest)
    {
        if (questNoticeList.Count > maxquestNoticeCount)
            return false;

        questNoticeList.Add(Managers.Game._playScene.SetQuestNoticeBar(quest));

        GetText((int)Texts.QuestNoticeCountText).text = questNoticeList.Count + " / " + maxquestNoticeCount;

        return true;
    }

    // 씬 퀘스트 알람 끄기
    public void CloseQuestNotice(QuestData quest)
    {
        foreach(UI_QuestNoticeSlot questNoticeSlot in questNoticeList)
        {
            if (questNoticeSlot._quest == quest)
            {
                questNoticeList.Remove(questNoticeSlot);
                Managers.Resource.Destroy(questNoticeSlot.gameObject);
                break;
            }
        }

        GetText((int)Texts.QuestNoticeCountText).text = questNoticeList.Count + " / " + maxquestNoticeCount;
    }

    public void RefreshUI()
    {
        // 현재 퀘스트 확인
        Managers.Game.RefreshQuest();

        GetText((int)Texts.QuestNoticeCountText).text = questNoticeList.Count + " / " + maxquestNoticeCount;

        if (Managers.Game.CurrentQuest.Count == 0)
            GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        foreach(Transform child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 퀘스트 목록 채우기
        foreach(QuestData questData in Managers.Game.CurrentQuest)
        {
            UI_QuestSlot questSlot = Managers.UI.MakeSubItem<UI_QuestSlot>(parent: GetObject((int)Gameobejcts.Content).transform);
            questSlot.SetInfo(questData);
        }
        
        // 현재 첫 번째 퀘스트 정보
        if (Managers.Game.CurrentQuest.Count >= 1)
            OnQuest(Managers.Game.CurrentQuest[0]);
    }
}
