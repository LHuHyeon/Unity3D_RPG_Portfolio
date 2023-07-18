using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
[ 장비/스탯 Popup 스크립트 ]
1. 장비와 스탯을 관리하는 Popup이다.
2. 자주 호출되는 함수 : SetEquipment(아이템 슬롯)
2-1. SetEquipment(아이템 슬롯) : 장비를 장착할 수 있다.
*/

public class UI_EqStatPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Title,
        EqSlot,
        ExitButton,
        StatBackground,
    }

    enum Buttons
    {
        HpAddPointButton,
        MpAddPointButton,
        STRAddPointButton,
        LUKAddPointButton,
        StatButton,
    }

    enum Texts
    {
        StatPointText,
        HpStatPointText,
        MpStatPointText,
        STRStatPointText,
        LUKStatPointText,
        StatNameText,
        StatText,
    }

    public List<UI_ArmorItem> armorSlots;
    public UI_WeaponItem weaponSlot;

    bool isClickStatButton = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        armorSlots = new List<UI_ArmorItem>();

        popupType = Define.Popup.Equipment;

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        Managers.Input.KeyAction -= OnEquipmentUI;
        Managers.Input.KeyAction += OnEquipmentUI;

        SetInfo();

        Invoke("DelayInit", 0.0001f);

        return true;
    }
    void DelayInit() { Managers.UI.ClosePopupUI(this); }

    void Update()
    {
        if (Managers.Game.isPopups[Define.Popup.Equipment] == true)
            RefreshUI();
    }

    // 장비/스탯 창 활성화
    void OnEquipmentUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.Game.isPopups[Define.Popup.Equipment] = !Managers.Game.isPopups[Define.Popup.Equipment];

            if (Managers.Game.isPopups[Define.Popup.Equipment])
                Managers.UI.OnPopupUI(this);
            else
                Exit();
        }
    }

    // 장비 장착
    public void SetEquipment(UI_SlotItem itemSlot)
    {
        // 무기, 장비 확인
        if (itemSlot.item.itemType == Define.ItemType.Armor)
        {
            ArmorItemData armor = itemSlot.item as ArmorItemData;

            // 장비 부위 체크
            foreach(UI_ArmorItem armorSlot in armorSlots)
            {
                // 같은 장비면 장착 or 체인지
                if (armorSlot.armorType == armor.armorType)
                {
                    armorSlot.ChangeArmor(itemSlot);
                    break;
                }
            }
        }
        else if (itemSlot.item.itemType == Define.ItemType.Weapon)
        {
            weaponSlot.ChangeWeapon(itemSlot);
        }
    }

    public void SetInfo()
    {
        // 버튼 클릭 적용
        GetButton((int)Buttons.HpAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.HpAddPointButton); });
        GetButton((int)Buttons.MpAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.MpAddPointButton); });
        GetButton((int)Buttons.STRAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.STRAddPointButton); });
        GetButton((int)Buttons.LUKAddPointButton).onClick.AddListener(()=>{ AddStat(Buttons.LUKAddPointButton); });

        GetButton((int)Buttons.StatButton).onClick.AddListener(OnClickStatButton);
        GetObject((int)Gameobjects.StatBackground).SetActive(false);

        string statNameText = 
$@"<color=white>이름</color>
<color=yellow>Lv.</color>
<color=white>체력</color>
<color=white>마나</color>
<color=white>이동속도</color>
<color=red>공격력</color>
<color=blue>방어력</color>";

        GetText((int)Texts.StatNameText).text = statNameText;

        SetEventHandler();
    }

    void RefreshUI()
    {
        GetText((int)Texts.StatPointText).text = Managers.Game.StatPoint.ToString();
        GetText((int)Texts.HpStatPointText).text = Managers.Game.HpPoint.ToString();
        GetText((int)Texts.MpStatPointText).text = Managers.Game.MpPoint.ToString();
        GetText((int)Texts.STRStatPointText).text = Managers.Game.STR.ToString();
        GetText((int)Texts.LUKStatPointText).text = Managers.Game.LUK.ToString();

        string statText = 
$@"<color=white>{Managers.Game.Name}</color>
<color=white>{Managers.Game.Level}</color>
<color=white>{Managers.Game.MaxHp} {(Managers.Game.addHp != 0 ? $"(+{Managers.Game.addHp})":"")}</color>
<color=white>{Managers.Game.MaxMp} {(Managers.Game.addMp != 0 ? $"(+{Managers.Game.addMp})":"")}</color>
<color=white>{Managers.Game.MoveSpeed} {(Managers.Game.addMoveSpeed != 0 ? $"(+{Managers.Game.addMoveSpeed})":"")}</color>
<color=white>{Managers.Game.Attack}</color>
<color=white>{Managers.Game.Defense}</color>";

        GetText((int)Texts.StatText).text = statText;
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

    void OnClickStatButton()
    {
        isClickStatButton = !isClickStatButton;

        GetObject((int)Gameobjects.StatBackground).SetActive(isClickStatButton);
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
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
