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

        Managers.Resource.Destroy(gameObject);

        return true;
    }

    public void OnSlotTip(bool isActive)
    {
        if (isActive)
            Managers.UI.OnPopupUI(this);
        else
            Managers.Resource.Destroy(gameObject);
    }

    // 아이템 정보 확인시 새로고침
    public void RefreshUI(ItemData item)
    {
        if (item == null)
        {
            Debug.Log("아이템 정보가 없습니다.");
            OnSlotTip(false);
            return;
        }
        
        Managers.UI.SetCanvas(gameObject);

        GetImage((int)Images.ItemImage).sprite = item.itemIcon;

        GetText((int)Texts.ItemNameText).text = item.itemName;
        GetText((int)Texts.ItemTypeText).text = item.itemType.ToString();
        GetText((int)Texts.ItemGradeText).text = item.itemGrade.ToString();
        if (item.itemType == Define.ItemType.Use)
        {
            GetText((int)Texts.ItemLevelText).text = "";
            GetText((int)Texts.ItemStatText).text = item.itemDesc;
        }
        else if (item.itemType == Define.ItemType.Armor)
        {
            ArmorItemData armor = item as ArmorItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + armor.minLevel.ToString();

            string statStr = "";
            statStr += armor.defnece > 0 ? $"방어력 {armor.defnece}\n" : "";
            statStr += armor.hp > 0 ? $"체력 {armor.hp}\n" : "";
            statStr += armor.mp > 0 ? $"마나 {armor.mp}\n" : "";
            statStr += armor.moveSpeed > 0 ? $"이동속도 {armor.moveSpeed}\n" : "";

            GetText((int)Texts.ItemStatText).text = statStr;
        }
        else if (item.itemType == Define.ItemType.Weapon)
        {
            WeaponItemData weapon = item as WeaponItemData;
            GetText((int)Texts.ItemLevelText).text = "최소레벨 " + weapon.minLevel.ToString();
            GetText((int)Texts.ItemStatText).text = $"공격력 {weapon.attack.ToString()}";
        }
    }

    // 스킬 정보 확인시 새로고침
    public void RefreshUI(SkillData skill)
    {

    }
}
