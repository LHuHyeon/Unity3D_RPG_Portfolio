using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class UI_QuestSlot : UI_Base
{
    public QuestData _quest;
    public TextMeshProUGUI slotText;
    public Button sceneButton;
    public GameObject okButtonIcon;
    public bool isNotice = true;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            Managers.Game._playScene._quest.OnQuest(_quest);
        });

        sceneButton.onClick.AddListener(OnClickSceneButton);

        okButtonIcon.SetActive(!isNotice);

        return true;
    }

    public void SetInfo(QuestData quest)
    {
        _quest = quest;
        slotText.text = _quest.titleName;
    }

    // 씬에 퀘스트 알림 추가
    void OnClickSceneButton()
    {
        isNotice = !isNotice;

        if (isNotice == true)
            isNotice = Managers.Game._playScene._quest.SetQuestNotice(_quest);
        else
            Managers.Game._playScene._quest.CloseQuestNotice(_quest);

        okButtonIcon.SetActive(!isNotice);
    }
}
