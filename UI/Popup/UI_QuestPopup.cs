using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 퀘스트 Popup 스크립트 ]
1. 퀘스트를 확인할 수 있는 Popup이다.
2. 자주 호출되는 함수 : SetQeust(), OnQuest(), SetQuestNotice(), CloseQuestNotice()
2-1. SetQuest() : 새로운 퀘스트를 수락할 때 사용된다.
2-2. OnQuest() : 퀘스트 목록에 퀘스트를 눌러 정보를 확인할 때 사용된다.
2-3. SetQuestNotice() : 퀘스트 알림을 Scene에 추가할 때 사용된다.
2-4. CloseQuestNotice() : 퀘스트 알림을 끄고 싶을 때 사용된다.
*/

/*
 * File :   UI_QuestPopup.cs
 * Desc :   현재 수락한 퀘스트 확인 Popup
 *
 & Functions
 &  [Public]
 &  : Init()                - 초기 설정
 &  : SetQeust()            - 새 퀘스트 받기
 &  : QuestTargetCount()    - 퀘스트 목표 개수 반영
 &  : OnQuest()             - 퀘스트 목록 누를 시 퀘스트 정보 활성화
 &  : SetQuestNotice()      - Scene UI 알람 활성화
 &  : CloseQuestNotice()    - Scene UI 알람 비활성화
 &
 &  [Private]
 &  : OnQuestPopup()        - 퀘스트창 활성화or비활성화
 &  : SetInfo()             - 기본 설정
 &  : RefreshUI()           - 새로고침 UI
 *
 */

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

    public List<UI_QuestNoticeSlot> questNoticeList;    // Scene UI에 등록된 퀘스트 List

    private QuestData   currentClickQuest;              // 현재 클릭한 퀘스트 
    private int         maxquestNoticeCount = 5;        // 퀘스트 알림 최대 개수

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        popupType = Define.Popup.Quest;
        questNoticeList = new List<UI_QuestNoticeSlot>();
        
        // 자식 객체 불러오기
        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // InputManager에 입력 등록
        Managers.Input.KeyAction -= OnQuestPopup;
        Managers.Input.KeyAction += OnQuestPopup;

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        // 퀘스트창이 활성화되면 퀘스트 목표 실시간 새로고침
        if (Managers.Game.isPopups[Define.Popup.Quest] == true && currentClickQuest != null)
        {
            string str = currentClickQuest.targetDescription + "\n" + currentClickQuest.currnetTargetCount + " / " + currentClickQuest.targetCount;
            GetText((int)Texts.QuestTargetText).text = str;
        }
    }

    // 새로운 퀘스트 받기
    public void SetQeust(QuestData quest)
    {
        quest.isAccept = true;
        Managers.Game.CurrentQuest.Add(quest);

        RefreshUI();
        SetQuestNotice(quest);
    }

    // 퀘스트 목표 개수 반영
    public void QuestTargetCount(GameObject go)
    {
        if (Managers.Game.CurrentQuest.Count == 0)
            return;

        if (go.GetComponent<MonsterStat>())
        {
            // 퀘스트 조건과 맞는지 id 확인
            foreach(QuestData questData in Managers.Game.CurrentQuest)
            {
                if (questData.targetId == go.GetComponent<MonsterStat>().Id)
                {
                    questData.currnetTargetCount++;
                    if (questData.currnetTargetCount == questData.targetCount)
                    {
                        string message = $"퀘스트 완료!\n<color=yellow>[{questData.titleName}]</color>\n\n\n\n\n\n\n\n\n";
                        Managers.UI.MakeSubItem<UI_Guide>().SetInfo(message, Color.green);
                    }
                        
                    return;
                }
            }
        }
    }

    // 퀘스트 목록을 누르면
    public void OnQuest(QuestData quest)
    {
        if (quest.IsNull() == true)
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
            UI_ItemSlot rewardItem = Managers.UI.MakeSubItem<UI_ItemSlot>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo();
            rewardItem.AddItem(Managers.Data.Item[quest.rewardItems[i].ItemId], quest.rewardItems[i].itemCount);
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

    // 퀘스트창 활성화
    private void OnQuestPopup()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Managers.Game.isPopups[Define.Popup.Quest] = !Managers.Game.isPopups[Define.Popup.Quest];

            // 퀘스트 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.Quest])
            {
                Managers.UI.OnPopupUI(this);
                RefreshUI();
            }
            else
                Managers.UI.ClosePopupUI(this);
        }
    }

    private void SetInfo()
    {
        GetButton((int)Buttons.ExitButton).onClick.AddListener(()=>{Managers.UI.ClosePopupUI(this);});

        // 미리보기 삭제
        foreach(Transform child in GetObject((int)Gameobejcts.Content).transform)
            Managers.Resource.Destroy(child.gameObject);

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        for(int i=0; i<Managers.Game.CurrentQuest.Count; i++)
            SetQuestNotice(Managers.Game.CurrentQuest[i]);
    }

    private void RefreshUI()
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
