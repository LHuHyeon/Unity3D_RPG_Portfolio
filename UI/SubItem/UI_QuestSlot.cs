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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            Managers.Game._playScene._quest.OnQuest(_quest);
        });

        sceneButton.onClick.AddListener(OnClickSceneButton);

        return true;
    }

    public void SetInfo(QuestData quest)
    {
        Debug.Log("questSlot");
        _quest = quest;
        slotText.text = _quest.titleName;
    }

    void OnClickSceneButton()
    {
        // TODO : 씬 알림 뜨게하기
    }
}
