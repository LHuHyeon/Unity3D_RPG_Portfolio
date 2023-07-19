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

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
                return;

            // 슬롯 우클릭
            if (Input.GetMouseButtonUp(1))
            {
                Managers.Game._playScene._inventory.AcquireItem(item);
                ClearSlot();
            }
        }, Define.UIEvent.Click);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (UI_DragSlot.instance.dragSlotItem.IsNull() == true)
                return;

            // 아이템을 버린 위치가 UI가 아니라면
            if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
            {
                // 아이템 버리기
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;
            
            // 자기 자신 확인
            if (dragSlot == this)
                return;

            // 인벤에서 온거라면 
            if ((dragSlot is UI_InvenItem) == true)
                ChangeSlot(dragSlot as UI_InvenItem);

        }, Define.UIEvent.Drop);

        Managers.Game._getSlotInteract -= GetSlotInteract;
        Managers.Game._getSlotInteract += GetSlotInteract;
    }

    // 우클릭 아이템 받기
    void GetSlotInteract(UI_InvenItem invenSlot)
    {
        if (this.gameObject.activeSelf == true)
            ChangeSlot(invenSlot);
    }

    void ChangeSlot(UI_InvenItem invenSlot)
    {
        // 장비가 아니라면
        if ((invenSlot.item is EquipmentData) == false)
            return;

        // 강화 슬롯에 아이템이 있다면 인벤으로 돌려 보내기
        if (item.IsNull() == false)
            Managers.Game._playScene._inventory.AcquireItem(item);

        EquipmentData equipment = invenSlot.item as EquipmentData;

        Managers.Game._playScene._upgrade.RefreshUI(equipment);
        AddItem(invenSlot.item);

        invenSlot.ClearSlot();
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        Managers.Game._playScene._upgrade.Clear();
    }
}
