using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

/*
 * File :   UI_QuestButton.cs
 * Desc :   UI_QuestPopup.cs에서 생성되며 퀘스트 정보를 활성화하는 버튼으로 사용
 *
 & Functions
 &  [Public]
 &  : Init()         - 아이템 추가
 &  : SetInfo()      - 기능 설정
 &
 &  [Private]
 &  : OnClickSceneNoticeButton()    - 클릭 시 Scene UI에 퀘스트 알람 추가
 *
 */

public class UI_QuestButton : UI_Base
{
    enum Buttons    { QuestSceneButton }
    enum Images     { QuestSceneOkIcon }
    enum Texts      { QuestSlotText }

    private QuestData           _quest;

    private string              slotText;           // 퀘스트 제목
    private bool                isNotice = true;    // 퀘스트 알람

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            Managers.Game._playScene._quest.OnQuest(_quest);
        });

        GetButton((int)Buttons.QuestSceneButton).onClick.AddListener(OnClickSceneNoticeButton);

        GetImage((int)Images.QuestSceneOkIcon).gameObject.SetActive(!isNotice);

        GetText((int)Texts.QuestSlotText).text = slotText;

        return true;
    }

    public void SetInfo(QuestData quest)
    {
        _quest = quest;
        slotText = _quest.titleName;
    }

    // 씬에 퀘스트 알림 추가
    private void OnClickSceneNoticeButton()
    {
        isNotice = !isNotice;

        // 알람 활성화/비활성화
        if (isNotice == true)
            isNotice = Managers.Game._playScene._quest.SetQuestNotice(_quest);
        else
            Managers.Game._playScene._quest.CloseQuestNotice(_quest);

        GetImage((int)Images.QuestSceneOkIcon).gameObject.SetActive(!isNotice);
    }
}
