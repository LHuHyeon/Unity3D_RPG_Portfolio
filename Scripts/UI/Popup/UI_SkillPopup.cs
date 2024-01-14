using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   UI_SkillPopup.cs
 * Desc :   스킬 슬롯을 관리하는 Popup UI
 *
 & Functions
 &  [Public]
 &  : Init()            - 초기 설정
 &
 &  [Private]
 &  : OnSkillPopup()    - 스킬창 활성화or비활성화
 &  : SetInfo()         - 기본 설정
 &  : Exit()            - 나가기 (초기화)
 *
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

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));

        popupType = Define.Popup.SkillUI;

        Managers.Input.KeyAction -= OnSkillPopup;
        Managers.Input.KeyAction += OnSkillPopup;

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    // 스킬창 활성화
    private void OnSkillPopup()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Managers.Game.isPopups[Define.Popup.SkillUI] = !Managers.Game.isPopups[Define.Popup.SkillUI];

            // 스킬창 Popup On/Off
            if (Managers.Game.isPopups[Define.Popup.SkillUI])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    private void SetInfo()
    {
        // Title 잡고 인벤토리 이동
        RectTransform skillPopupPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData) =>
        {
            skillPopupPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(skillPopupPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(skillPopupPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData) =>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData) =>
        {
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    private void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
