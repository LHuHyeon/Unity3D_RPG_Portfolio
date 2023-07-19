using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 스킬 Popup 스크립트 ]
1. 레벨에 따른 스킬을 흭득할 수 Popup이다.
2. 해당 클래스는 Popup만 활성화/비활성화 해주면 그 안의 슬롯들이 기능을 담당하고 있다.
3. 슬롯 : UI_SkillItem.cs(스킬 슬롯), UI_SkillBarItem.cs(스킬 등록 슬롯)
*/

public class UI_SkillPopup : UI_Popup
{
    enum Gameobjects
    {
        Title,
        Background,
        ExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
    
        BindObject(typeof(Gameobjects));

        popupType = Define.Popup.SkillUI;

        Managers.Input.KeyAction -= OnSkillPopup;
        Managers.Input.KeyAction += OnSkillPopup;

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void OnSkillPopup()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Managers.Game.isPopups[Define.Popup.SkillUI] = !Managers.Game.isPopups[Define.Popup.SkillUI];

            if (Managers.Game.isPopups[Define.Popup.SkillUI])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    void SetInfo()
    {
        // Title 잡고 인벤토리 이동
        RectTransform skillPopupPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            skillPopupPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(skillPopupPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(skillPopupPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
