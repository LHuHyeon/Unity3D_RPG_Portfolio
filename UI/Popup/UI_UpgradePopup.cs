using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_UpgradePopup : UI_Popup
{
    enum Gameobjects
    {
        ItemSlot,
    }

    enum Buttons
    {
        UpgradeButton,
        ExitButton,
    }

    enum Images
    {
        ItemIcon,
    }

    enum Texts
    {
        ItemNameText,
        UpgradeResultText,
        UpgradeGoldText,
    }

    public EquipmentData _equipment;

    int maxUpgradeCount = 10;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void SetInfo()
    {
        GetText((int)Texts.ItemNameText).text = "강화할 장비를 선택하세요";
        GetText((int)Texts.UpgradeResultText).text = "";
        GetText((int)Texts.UpgradeGoldText).text = "0";

        GetButton((int)Buttons.UpgradeButton).onClick.AddListener(OnClickUpgradeButton);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(ExitUpgrade);
    }

    public void RefreshUI(EquipmentData equipment)
    {
        _equipment = equipment;

        // 풀강 확인
        if (equipment.upgradeCount >= maxUpgradeCount)
        {
            GetText((int)Texts.ItemNameText).text = _equipment.itemName;
            GetText((int)Texts.UpgradeResultText).text = $"Max";
            GetText((int)Texts.UpgradeGoldText).text = "";
        }
        else
        {
            GetText((int)Texts.ItemNameText).text = _equipment.itemName;
            GetText((int)Texts.UpgradeResultText).text = $"{_equipment.upgradeCount}   →   {_equipment.upgradeCount+1}";
            GetText((int)Texts.UpgradeGoldText).text = Managers.Game.EquipmentUpgradeGold(_equipment).ToString();
        }
    }

    void OnClickUpgradeButton()
    {
        if (_equipment == null)
            return;

        if (_equipment.upgradeCount >= maxUpgradeCount)
            return;

        // 금액 확인
        int upgradeGold = Managers.Game.EquipmentUpgradeGold(_equipment);
        if (Managers.Game.Gold < upgradeGold)
        {
            GetText((int)Texts.ItemNameText).text = "금액이 부족합니다!";
            return;
        }

        Managers.Game.Gold -= upgradeGold;

        // 강화 적용
        Managers.Game.EquipmentUpgrade(_equipment);
        RefreshUI(_equipment);
    }

    public void Clear()
    {
        _equipment = null;

        Managers.Game._playScene._slotTip.OnSlotTip(false);

        GetText((int)Texts.ItemNameText).text = "강화할 장비를 선택하세요";
        GetText((int)Texts.UpgradeResultText).text = "";
        GetText((int)Texts.UpgradeGoldText).text = "0";
    }

    public void ExitUpgrade()
    {
        if (_equipment != null)
            Managers.Game._playScene._inventory.AcquireItem(_equipment);

        Clear();

        GetObject((int)Gameobjects.ItemSlot).GetComponent<UI_UpgradeItem>().ClearSlot();

        Managers.Game.IsInteract = false;
        
        Managers.UI.CloseAllPopupUI();
    }
}
