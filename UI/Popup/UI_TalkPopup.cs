using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TalkPopup : UI_Popup
{
    enum Gameobejcts
    {
        TalkBackground,
        QuestJournal,
        QuestRewardGrid,
    }

    enum Buttons
    {
        NextButton,
        RefusalButton,
        AcceptButton,
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

    TalkData talkData;
    QuestData questData;

    [SerializeField]
    float delayTime = 0.1f;

    int nextIndex = 0;

    bool isNext = false;        // 다음 대화로 넘어갈 수 있는지
    bool isNextTalk = false;    // 다음 대화가 있는지

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        popupType = Define.Popup.Talk;

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        GetButton((int)Buttons.NextButton).onClick.AddListener(OnClickNextButton);
        GetButton((int)Buttons.RefusalButton).onClick.AddListener(OnClickRefusalButton);
        GetButton((int)Buttons.AcceptButton).onClick.AddListener(OnClickAcceptButton);

        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Space))
        {
            // 말이 다 안 끝났다면
            if (isNext == false)
            {
                // 대화 속도 빠르게
                delayTime = delayTime / 2;
                return;
            }

            // 퀘스트가 On이라면
            if (GetObject((int)Gameobejcts.QuestJournal).activeSelf == true)
                return;

            // 다음 대화가 있으면 진행
            if (isNextTalk == true)
                OnClickNextButton();
            else
                Clear();
        }
    }

    // 일반 대화 세팅
    public void SetInfo(string text, string npcName=null)
    {
        if (text != null)
        {
            if (npcName != null)
                GetText((int)Texts.NameText).text = npcName;

            // 대화 진행 후 종료
            isNextTalk = false;
            StartCoroutine(TypingText(text));
            return;
        }
    }

    // 퀘스트 대화 세팅
    public void SetInfo(TalkData talk, QuestData quest, string npcName=null)
    {
        if (talk == null || quest == null)
        {
            Debug.Log("talk or quest Data Null");
            return;
        }

        if (npcName != null)
            GetText((int)Texts.NameText).text = npcName;

        talkData = talk;
        questData = quest;

        nextIndex = 0;

        SetQuestUI();
        NextTalk();
    }

    public void NextTalk()
    {
        // 할 대화가 없으면 종료
        if (nextIndex >= talkData.questStartTalk.Count)
        {
            Clear();
            return;
        }

        // 대화 시작
        StartCoroutine(TypingText(talkData.questStartTalk[nextIndex]));

        nextIndex++;

        // 다음 대화가 없으면 퀘스트 On
        if (nextIndex >= talkData.questStartTalk.Count)
        {
            isNextTalk = false;
            GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
        }
        else
            isNextTalk = true;
    }

    // 타이핑 모션 코루틴
    IEnumerator TypingText(string sentence)
    {
        GetText((int)Texts.TalkText).text = "";

        isNext = false;
        delayTime = 0.05f;

        // 대화 타이밍 모션 실행
        foreach(var letter in sentence)
        {
            GetText((int)Texts.TalkText).text += letter;
            yield return new WaitForSeconds(delayTime);
        }

        isNext = true;

        // 다음 대화가 있다면 다음 버튼 On
        if (isNextTalk == true)
            GetButton((int)Buttons.NextButton).gameObject.SetActive(true);
        
        // 퀘스트가 켜지면 수락 or 거절 버튼 On
        if (GetObject((int)Gameobejcts.QuestJournal).activeSelf == true)
            IsQuestActive(true);
    }

    // 다음 버튼
    void OnClickNextButton()
    {
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        NextTalk();
    }

    // 거절 버튼
    void OnClickRefusalButton()
    {
        IsQuestActive(false);
        SetInfo(talkData.refusalTalk);
    }

    // 수락 버튼
    void OnClickAcceptButton()
    {
        Managers.Game.CurrentQuest.Add(questData);
        Managers.Game._playScene._quest.RefreshUI();
        Managers.Game._playScene._quest.SetQuestNotice(questData);
        questData.isAccept = true;

        IsQuestActive(false);
        SetInfo(talkData.acceptTalk);
    }

    void IsQuestActive(bool isTrue)
    {
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(isTrue);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(isTrue);
        GetObject((int)Gameobejcts.QuestJournal).SetActive(isTrue);
    }

    void SetQuestUI()
    {
        GetText((int)Texts.QuestTitleText).text = questData.titleName;
        GetText((int)Texts.QuestDescText).text = questData.description;
        GetText((int)Texts.QuestTargetText).text = questData.targetDescription;
        GetText((int)Texts.QuestRewardGoldText).text = questData.rewardGold.ToString();
        GetText((int)Texts.QuestRewardExpText).text = questData.rewardExp.ToString();

        foreach(Transform child in GetObject((int)Gameobejcts.QuestRewardGrid).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<questData.rewardItems.Count; i++)
        {
            UI_RewardItem rewardItem = Managers.UI.MakeSubItem<UI_RewardItem>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo(Managers.Data.Item[questData.rewardItems[i].ItemId], questData.rewardItems[i].itemCount);
        }
    }

    public void Clear()
    {
        Managers.Game.IsInteract = false;
        Managers.Game.isPopups[Define.Popup.Talk] = false;

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);
    }
}
