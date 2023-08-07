using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
[ 업그레이드 Slot 스크립트 ]
1. 업그레이드 Popup의 slot이다.
2. 인벤으로 부터 장비를 받을 수 있고, 강화를 할 수 있다.
*/

public class UI_UpgradeItem : UI_ItemDragSlot
{
    public override void SetInfo()
    {
        base.SetInfo();

        Managers.Game._getSlotInteract -= GetSlotInteract;
        Managers.Game._getSlotInteract += GetSlotInteract;
    }

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            return;

        // 슬롯 우클릭
        if (Input.GetMouseButtonUp(1))
        {
            Managers.Game._playScene._inventory.AcquireItem(item);
            ClearSlot();
        }
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            Managers.Game._playScene._inventory.AcquireItem(item);
            ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;
            
        // 자기 자신 확인
        if (dragSlot == this)
            return;

        ChangeSlot(dragSlot as UI_ItemSlot);
    }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 장비가 아니라면
        if ((itemSlot.item is EquipmentData) == false)
            return;

        // 강화 슬롯에 아이템이 있다면 인벤으로 돌려 보내기
        if (item.IsNull() == false)
            Managers.Game._playScene._inventory.AcquireItem(item);

        EquipmentData equipment = itemSlot.item as EquipmentData;

        Managers.Game._playScene._upgrade.RefreshUI(equipment);
        AddItem(itemSlot.item);

        (itemSlot as UI_InvenItem).ClearSlot();
    }

    // 우클릭 아이템 받기
    void GetSlotInteract(UI_InvenItem invenSlot)
    {
        if (Managers.Game._playScene._upgrade.gameObject.activeSelf == true)
            ChangeSlot(invenSlot);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        Managers.Game._playScene._upgrade.Clear();
    }
}
