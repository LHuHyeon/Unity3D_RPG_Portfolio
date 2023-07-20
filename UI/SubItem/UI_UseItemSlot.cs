using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_UseItemSlot : UI_ItemDragSlot
{
    enum Texts { KeyText, }

    public int key;
    string keyText;

    public override void SetInfo()
    {
        keyText = key.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(key) == true)
            AddItem(Managers.Game.UseItemBarList[key]);

        base.SetInfo();

        GetText((int)Texts.KeyText).text = keyText;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            Managers.Game._playScene._inventory.AcquireItem(item, itemCount);
            ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면
            if (dragSlot == this)
                return;

            ChangeSlot(dragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 같은 종류의 슬롯이거나 인벤 슬롯일 때 통과
        if ((itemSlot is UI_UseItemSlot) == false || (itemSlot is UI_InvenItem) == false)
            return;

        // 소비 아이템 확인
        if ((itemSlot.item is UseItemData) == false)
            return;

        // 지금 슬롯에 아이템이 존재할 때
        if (item.IsNull() == false)
        {
            // 아이디가 다를 경우 인벤으로 보내기
            if (item.id != itemSlot.item.id)
                Managers.Game._playScene._inventory.AcquireItem(item, itemCount);
        }

        AddItem(itemSlot.item, (itemSlot.item as UseItemData).itemCount);

        // 기존에 온 슬롯 삭제시키기 
        if (itemSlot is UI_UseItemSlot)
            (itemSlot as UI_UseItemSlot).ClearSlot();

        if (itemSlot is UI_InvenItem)
            (itemSlot as UI_InvenItem).ClearSlot();
    }

    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        if (Managers.Game.UseItemBarList.ContainsKey(key) == false)
            Managers.Game.UseItemBarList.Add(key, _item as UseItemData);
        else
            Managers.Game.UseItemBarList[key] = _item as UseItemData;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.UseItemBarList[key] = null;
    }
}
