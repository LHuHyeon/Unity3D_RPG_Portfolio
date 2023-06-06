using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TalkPopup : UI_Popup
{
    /*
    1. 대화 진행 (이름, 내용)
    2. 퀘스트 띄우기
    3. 다음 버튼
    */

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

    string talkText;

    [SerializeField]
    float delayTime = 0.1f;

    int nextIndex = 0;

    bool isNext = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobejcts));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetText((int)Texts.TalkText).text = talkText;

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
            if (isNext == true)
                NextTalk();
            else
                Clear();
        }
    }

    // 퀘스트 대화 세팅
    public void SetInfo(TalkData talk, QuestData quest)
    {
        if (talk == null || quest == null)
        {
            Debug.Log("talk or quest Data Null");
            return;
        }

        talkData = talk;
        questData = quest;

        nextIndex = 0;

        NextTalk();
    }

    // 일반 대화 세팅
    public void SetInfo(string text)
    {
        if (text != null)
        {
            // 대화 진행 후 종료
            StartCoroutine(TypingText(text));
            return;
        }
    }

    public void NextTalk()
    {
        // 할 대화가 없으면 종료
        if (nextIndex > talkData.questStartTalk.Count)
        {
            Clear();
            return;
        }

        // 대화 시작
        StartCoroutine(TypingText(talkData.questStartTalk[nextIndex]));

        nextIndex++;

        // 다음 대화가 없으면 퀘스트 On
        if (nextIndex > talkData.questStartTalk.Count)
        {
            isNext = false;
            GetObject((int)Gameobejcts.QuestJournal).SetActive(true);
        }
        else
            isNext = true;
    }

    // 타이핑 모션 코루틴
    IEnumerator TypingText(string sentence)
    {
        // 대화 타이밍 모션 실행
        foreach(var letter in sentence)
        {
            talkText += letter;
            yield return new WaitForSeconds(delayTime);
        }

        // 다음 대화가 있다면 다음 버튼 On
        if (isNext == true)
        {
            GetButton((int)Buttons.NextButton).gameObject.SetActive(true);
        }
        
        // 퀘스트가 켜지면 수락 or 거절 버튼 On
        if (GetObject((int)Gameobejcts.QuestJournal).activeSelf == true)
        {

        }
    }

    public void Clear()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
