using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * File :   UI_TalkPopup.cs
 * Desc :   NPC와 대화할 때 사용되는 Popup UI
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : SetInfo()     - 대화 기능 설정
 &  : SetInfo()     - 퀘스트 대화 기능 설정
 &  : Clear()       - 초기화
 &
 &  [Private]
 &  : Update()                  - 키 입력으로 대화 속도 증가 및 다음 대화 진행
 &  : NextTalk()                - 대화 진행
 &  : TypingText()              - 대화 출력 Coroutine
 &  : OnClickNextButton()       - 다음 대화 버튼
 &  : OnClickRefusalButton()    - 퀘스트 거절 버튼
 &  : OnClickAcceptButton()     - 퀘스트 수락 버튼
 &  : IsQuestActive()           - 퀘스트 관련 객체 활성화 여부
 &  : SetQuestUI()              - 퀘스트 정보 설정
 *
 */

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

    private TalkData    talkData;               // 대화 데이터
    private QuestData   questData;              // 퀘스트 데이터

    private int         nextTalkIndex = 0;

    private bool        isNext = false;         // 다음 대화로 넘어갈 수 있는지
    private bool        isNextTalk = false;     // 다음 대화가 있는지

    [SerializeField]
    private float       talkDelayTime = 0.1f;   // 대화 속도 딜레이  

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        popupType = Define.Popup.Talk;

        // 자식 객체 가져오기
        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        // 퀘스트 정보 비활성화
        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);

        // 버튼 기능 등록
        GetButton((int)Buttons.NextButton).onClick.AddListener(OnClickNextButton);
        GetButton((int)Buttons.RefusalButton).onClick.AddListener(OnClickRefusalButton);
        GetButton((int)Buttons.AcceptButton).onClick.AddListener(OnClickAcceptButton);

        // 버튼 비활성화
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        // 상호작용 키, 스페이스 바, 마우스를 좌클릭하면 대화속도가 빨라지고 대화를 넘김.
        if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // 말이 다 안 끝났다면
            if (isNext == false)
            {
                // 대화 속도 빠르게
                talkDelayTime = talkDelayTime / 2;
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
        if (text.IsNull() == false)
        {
            if (npcName.IsNull() == false)
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
        if (talk.IsNull() == true || quest.IsNull() == true)
        {
            Debug.Log("talk or quest Data Null");
            return;
        }

        if (npcName.IsNull() == false)
            GetText((int)Texts.NameText).text = npcName;

        talkData = talk;
        questData = quest;

        nextTalkIndex = 0;

        SetQuestUI();       // 퀘스트 정보 설정
        NextTalk();         // 대화 시작
    }

    private void NextTalk()
    {
        // 할 대화가 없으면 종료
        if (nextTalkIndex >= talkData.questStartTalk.Count)
        {
            Clear();
            return;
        }

        // 대화 시작
        StartCoroutine(TypingText(talkData.questStartTalk[nextTalkIndex]));

        nextTalkIndex++;

        // 다음 대화가 없으면 퀘스트 정보 활성화
        if (nextTalkIndex >= talkData.questStartTalk.Count)
        {
            isNextTalk = false;
            GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
        }
        else
            isNextTalk = true;
    }

    // 타이핑 모션 코루틴
    private IEnumerator TypingText(string sentence)
    {
        GetText((int)Texts.TalkText).text = "";

        isNext = false;
        talkDelayTime = 0.05f;

        // 대화 타이밍 모션 실행
        foreach(var letter in sentence)
        {
            GetText((int)Texts.TalkText).text += letter;
            yield return new WaitForSeconds(talkDelayTime);
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
    private void OnClickNextButton()
    {
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        NextTalk();
    }

    // 거절 버튼
    private void OnClickRefusalButton()
    {
        IsQuestActive(false);
        SetInfo(talkData.refusalTalk);
    }

    // 수락 버튼
    private void OnClickAcceptButton()
    {
        Managers.Game._playScene._quest.SetQeust(questData);

        IsQuestActive(false);
        SetInfo(talkData.acceptTalk);

        Managers.UI.MakeWorldSpaceUI<UI_Navigation>().SetInfo(questData.targetPos);
    }

    // 퀘스트 활성화/비활성화
    private void IsQuestActive(bool isTrue)
    {
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(isTrue);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(isTrue);
        GetObject((int)Gameobejcts.QuestJournal).SetActive(isTrue);
    }

    // 퀘스트 정보 설정
    private void SetQuestUI()
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
            UI_ItemSlot rewardItem = Managers.UI.MakeSubItem<UI_ItemSlot>(parent: GetObject((int)Gameobejcts.QuestRewardGrid).transform);
            rewardItem.SetInfo();
            rewardItem.AddItem(Managers.Data.Item[questData.rewardItems[i].ItemId], questData.rewardItems[i].itemCount);
        }
    }

    public void Clear()
    {
        Managers.Game.IsInteract = false;

        GetObject((int)Gameobejcts.QuestJournal).SetActive(false);
        GetButton((int)Buttons.NextButton).gameObject.SetActive(false);
        GetButton((int)Buttons.RefusalButton).gameObject.SetActive(false);
        GetButton((int)Buttons.AcceptButton).gameObject.SetActive(false);

        Managers.UI.ClosePopupUI(this);
    }
}
