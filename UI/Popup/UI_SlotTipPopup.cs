using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SlotTipPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
    }

    enum Images
    {
        ItemImage,
    }

    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemGradeText,
        ItemLevelText,
        ItemStatText,
    }

    public RectTransform background;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        background = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void OnSlotTip(bool isActive)
    {
        if (isActive)
            Managers.UI.OnPopupUI(this);
        else
        {
            if (this.gameObject.activeSelf == true)
                Managers.UI.ClosePopupUI(this);
        }
    }

    // 아이템 정보 확인시 새로고침
    public void RefreshUI(ItemData item)
    {
        if (item.IsNull() == true)
        {
            Debug.Log("아이템 정보가 없습니다.");
            OnSlotTip(false);
            return;
        }
        
        Managers.UI.SetCanvas(gameObject);

        // 위치 설정
        RectTransform tipRect = background;
        Vector3 slotTipPos = background.anchoredPosition;
        slotTipPos.x = slotTipPos.x + (tipRect.rect.width * 0.65f);
        slotTipPos.y = slotTipPos.y - (tipRect.rect.height * 0.65f); 
        background.anchoredPosition = slotTipPos;

        GetImage((int)Images.ItemImage).sprite = item.itemIcon;

        GetText((int)Texts.ItemNameText).text = item.itemName;
        GetText((int)Texts.ItemTypeText).text = item.itemType.ToString();
        GetText((int)Texts.ItemGradeText).text = item.itemGrade.ToString();

        // 아이템 등급에 따른 색깔
        switch(item.itemGrade)
        {
            case Define.itemGrade.Common:
                SetColor(Color.white);
                break;
            case Define.itemGrade.Rare:
                SetColor(Color.green);
                break;
            case Define.itemGrade.Epic:
                SetColor(Color.blue);
                break;
            case Define.itemGrade.Legendary:
                SetColor(Color.yellow);
                break;
        }

        // 장비라면 
        if (item is EquipmentData)
        {
            // 강화가 됐다면
            if ((item as EquipmentData).upgradeCount > 0)
                GetText((int)Texts.ItemNameText).text = item.itemName +  $" [+{(item as EquipmentData).upgradeCount}]";
        }    
        
        // 아이템 종류 별로 세팅
        if (item.itemType == Define.ItemType.Use)
        {
            GetText((int)Texts.ItemLevelText).text = "";
            GetText((int)Texts.ItemStatText).text = item.itemDesc;
        }
        else if (item.itemType == Define.ItemType.Armor)
        {
            ArmorItemData armor = item as ArmorItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + armor.minLevel;

            string statStr = "";
            // 강화 확인
            if (armor.upgradeCount > 0)
            {
                statStr += armor.defnece > 0 ? $"방어력 {armor.defnece} (+{armor.addDefnece})\n" : "";
                statStr += armor.hp > 0 ? $"체력 {armor.hp} (+{armor.addHp})\n" : "";
                statStr += armor.mp > 0 ? $"마나 {armor.mp} (+{armor.addMp})\n" : "";
            }
            else
            {
                statStr += armor.defnece > 0 ? $"방어력 {armor.defnece}\n" : "";
                statStr += armor.hp > 0 ? $"체력 {armor.hp}\n" : "";
                statStr += armor.mp > 0 ? $"마나 {armor.mp}\n" : "";
            }
            
            statStr += armor.moveSpeed > 0 ? $"이동속도 {armor.moveSpeed}\n" : "";

            GetText((int)Texts.ItemStatText).text = statStr;
        }
        else if (item.itemType == Define.ItemType.Weapon)
        {
            WeaponItemData weapon = item as WeaponItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + weapon.minLevel;

            // 강화 확인
            if (weapon.upgradeCount > 0)
                GetText((int)Texts.ItemStatText).text = $"공격력 {weapon.attack} (+{weapon.addAttack})";
            else
                GetText((int)Texts.ItemStatText).text = $"공격력 {weapon.attack}";
        }
    }

    void SetColor(Color color)
    {
        GetText((int)Texts.ItemNameText).color = color;
        GetText((int)Texts.ItemGradeText).color = color;
    }

    // 스킬 정보 확인시 새로고침
    public void RefreshUI(SkillData skill)
    {

    }
}
