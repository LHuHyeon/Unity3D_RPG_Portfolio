using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 아이템 이동 Slot 스크립트 ]
1. 드래그 드랍이 필요한 아이템 Slot이 상속받는다.
*/

public class UI_ItemDragSlot : UI_ItemSlot
{
    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        // 아이템이 존재할 시 마우스로 들기 가능.
        if (item.IsNull() == true)
            return;

        UI_DragSlot.instance.dragSlotItem = this;
        UI_DragSlot.instance.DragSetImage(icon);

        UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnDragSlot(PointerEventData eventData)
    {
        // 마우스 드래그 방향으로 아이템 이동
        if (item.IsNull() == false && UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        UI_DragSlot.instance.SetColor(0);
        UI_DragSlot.instance.dragSlotItem = null;
    }

    // 슬롯 바꾸기
    protected virtual void ChangeSlot(UI_ItemSlot itemSlot) {}
}
