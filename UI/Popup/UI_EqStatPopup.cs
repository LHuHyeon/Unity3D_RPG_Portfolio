using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EqStatPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Title,
        EqSlot,
        ExitButton,
    }

    enum Buttons
    {
        HpAddPointButton,
        MpAddPointButton,
        STRAddPointButton,
        LUKAddPointButton,
    }

    enum Texts
    {
        StatPointText,
        HpStatPointText,
        MpStatPointText,
        STRStatPointText,
        LUKStatPointText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        Managers.Input.KeyAction -= OnEquipmentUI;
        Managers.Input.KeyAction += OnEquipmentUI;

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void FixedUpdate()
    {
        if (Managers.Game.isEquipment == true)
            RefreshUI();
    }

    // 장비/스탯 창 활성화
    void OnEquipmentUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.Game.isEquipment = !Managers.Game.isEquipment;

            if (Managers.Game.isEquipment)
                Managers.UI.OnPopupUI(this);
            else
                Managers.UI.ClosePopupUI(this);
        }
    }

    public void SetInfo()
    {
        // 버튼 클릭 적용
        GetButton((int)Buttons.HpAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.HpAddPointButton); });
        GetButton((int)Buttons.MpAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.MpAddPointButton); });
        GetButton((int)Buttons.STRAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.STRAddPointButton); });
        GetButton((int)Buttons.LUKAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.LUKAddPointButton); });

        SetEventHandler();
    }

    // 스탯 포인트 적용
    void AddStat(Buttons stat)
    {
        if (Managers.Game.StatPoint == 0)
            return;

        switch(stat)
        {
            case Buttons.HpAddPointButton:
                Managers.Game.HpPoint++;
                break;
            case Buttons.MpAddPointButton:
                Managers.Game.MpPoint++;
                break;
            case Buttons.STRAddPointButton:
                Managers.Game.STR++;
                break;
            case Buttons.LUKAddPointButton:
                Managers.Game.LUK++;
                break;
        }

        Managers.Game.StatPoint--;
        RefreshUI();
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform eqStatPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            eqStatPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(eqStatPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(eqStatPos.anchoredPosition.y + eventData.delta.y, -253, 217)
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
            Managers.Game.isEquipment = false;
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {
        GetText((int)Texts.StatPointText).text = Managers.Game.StatPoint.ToString();
        GetText((int)Texts.HpStatPointText).text = Managers.Game.HpPoint.ToString();
        GetText((int)Texts.MpStatPointText).text = Managers.Game.MpPoint.ToString();
        GetText((int)Texts.STRStatPointText).text = Managers.Game.STR.ToString();
        GetText((int)Texts.LUKStatPointText).text = Managers.Game.LUK.ToString();
    }
}
