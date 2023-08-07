using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/*
[ 소비아이템 Slot 스크립트 ]
1. PlayScene 하단에 있는 소비아이템 slot이다.
2. 각 담당 key가 있고 아이템이 등록되면 player는 해당 key의 아이템을 사용할 수 있다.
*/

public class UI_UseItemSlot : UI_ItemDragSlot
{
    public int key;
    public TextMeshProUGUI keyText;

    public override void SetInfo()
    {
        base.SetInfo();

        keyText.text = key.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(key) == true)
        {
            UseItemData useItem = Managers.Game.UseItemBarList[key];
            AddItem(useItem, useItem.itemCount);
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Managers.Game._playScene._inventory.AcquireItem(item, itemCount) == true)
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

            // 같은 종류의 슬롯이거나 인벤 슬롯일 때 통과
            if ((dragSlot is UI_UseItemSlot) == true || (dragSlot is UI_InvenItem) == true)
                ChangeSlot(dragSlot as UI_ItemSlot);
        }
    }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 소비 아이템 확인
        if ((itemSlot.item is UseItemData) == false)
            return;

        // 지금 슬롯에 아이템이 존재할 때
        if (item.IsNull() == false)
        {
            // 아이디 확인 후 개수 증가 or 체인지
            if (item.id == itemSlot.item.id)
                SetCount((itemSlot.item as UseItemData).itemCount);
            else
            {
                if (Managers.Game._playScene._inventory.AcquireItem(item, itemCount) == false)
                    return;
                
                AddItem(itemSlot.item, (itemSlot.item as UseItemData).itemCount);
            }
        }
        else AddItem(itemSlot.item, (itemSlot.item as UseItemData).itemCount);

        // 기존에 온 슬롯 삭제시키기 
        if (itemSlot is UI_UseItemSlot) (itemSlot as UI_UseItemSlot).ClearSlot();
        if (itemSlot is UI_InvenItem) (itemSlot as UI_InvenItem).ClearSlot();
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
