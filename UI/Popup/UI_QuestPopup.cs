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

    enum Texts
    {
        QuestTitleText,
        QuestDescText,
        QuestTargetText,
        QuestRewardGoldText,
        QuestRewardExpText,
        QuestSceneCountText,
    }

    // PlayScene에 띄울 퀘스트 창 개수
    int maxQuestSceneCount = 5;
    List<QuestData> sceneQuest;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        sceneQuest = new List<QuestData>();

        Managers.Input.KeyAction -= OnQuestPopup;
        Managers.Input.KeyAction += OnQuestPopup;
        
        BindObject(typeof(Gameobejcts));
        BindText(typeof(Texts));

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
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

    public void RefreshUI()
    {
        // 현재 퀘스트 확인
        Managers.Game.RefreshQuest();

        foreach(Transform child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 퀘스트 목록 채우기
        foreach(QuestData questData in Managers.Game.CurrentQuest)
        {
            UI_QuestSlot questSlot = Managers.UI.MakeSubItem<UI_QuestSlot>(parent: GetObject((int)Gameobejcts.Content).transform);
            questSlot.SetInfo(questData);
        }
    }
}
