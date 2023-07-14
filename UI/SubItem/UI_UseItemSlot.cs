using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

[Serializable]
public class UI_UseItemSlot : UI_SlotItem
{
    public int key;
    public int itemCount;

    public TextMeshProUGUI keyText;
    public TextMeshProUGUI itemCountText;

    public override void SetInfo()
    {
        slotType = Define.SlotType.UseItem;
        keyText.text = key.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(key) == true)
            AddItem(Managers.Game.UseItemBarList[key]);

        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (UI_DragSlot.instance.dragSlotItem == null)
                return;

            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Managers.Game._playScene._inventory.AcquireItem(item, itemCount);
                ClearSlot();
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 소비 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            if (dragSlot != null)
            {
                // 자기 자신이라면
                if (dragSlot == this)
                    return;

                // 같은 종류의 슬롯이거나 인벤 슬롯일 때 통과
                if ((dragSlot is UI_UseItemSlot) == true || (dragSlot is UI_InvenItem) == true)
                    ChangeItemSlot(dragSlot);
            }

        }, Define.UIEvent.Drop);
    }

    void ChangeItemSlot(UI_SlotItem slot)
    {
        // 소비 아이템 확인
        if ((slot.item is UseItemData) == false)
            return;

        // 지금 슬롯에 아이템이 존재할 때
        if (item != null)
        {
            // 아이디가 다를 경우 인벤으로 보내기
            if (item.id != slot.item.id)
                Managers.Game._playScene._inventory.AcquireItem(item, itemCount);
        }

        AddItem(slot.item, (slot.item as UseItemData).itemCount);

        // 기존에 온 슬롯 삭제시키기 
        if (slot is UI_UseItemSlot)
            (slot as UI_UseItemSlot).ClearSlot();

        if (slot is UI_InvenItem)
            (slot as UI_InvenItem).ClearSlot();
    }

    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        itemCount = (item as UseItemData).itemCount;
        itemCountText.text = itemCount.ToString();

        if (Managers.Game.UseItemBarList.ContainsKey(key) == false)
            Managers.Game.UseItemBarList.Add(key, _item as UseItemData);
        else
            Managers.Game.UseItemBarList[key] = _item as UseItemData;
    }

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        itemCountText.text = itemCount.ToString();
        
        (item as UseItemData).itemCount += count;

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.UseItemBarList[key] = null;

        itemCount = 0;
        itemCountText.text = "";
    }
}
