using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            Managers.Game.isSkillUI = !Managers.Game.isSkillUI;

            if (Managers.Game.isSkillUI)
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    public void SetInfo()
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
            Managers.Game.isSkillUI = false;
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.Game.isSkillUI = false;
        Managers.UI.ClosePopupUI(this);
    }
}
