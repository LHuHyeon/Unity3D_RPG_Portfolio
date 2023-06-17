using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_UpgradeItem : UI_SlotItem
{
    public override void SetInfo()
    {
        slotType = Define.SlotType.Upgrade;
        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // 아이템 인벤으로 이동
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;
            
            // 장비가 아니라면
            if ((dragSlot.item is EquipmentData) == false || dragSlot == this)
                return;

            EquipmentData equipment = dragSlot.item as EquipmentData;

            // 강화 슬롯에 다른 아이템이 있다면 인벤으로 돌려 보내기
            if (item != null)
                Managers.Game._playScene._inventory.AcquireItem(item);

            Managers.Game._playScene._upgrade.RefreshUI(equipment);
            AddItem(dragSlot.item);

            (dragSlot as UI_InvenItem).ClearSlot();

        }, Define.UIEvent.Drop);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        Managers.Game._playScene._upgrade.Clear();
    }
}
