using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   UI_ItemDragSlot.cs
 * Desc :   Drag, Drop PointerEvent가 필요한 Slot이 상속 받는다.
 *
 & Functions
 &  [Protected]
 &  : OnBeginDragSlot()     - 드래그를 시작할 때 DragSlot 생성
 &  : OnDragSlot()          - 드래그 중일 때 드래그 방향으로 DragSlot 이동
 &  : OnEndDragSlot()       - 드래그가 끝나면 DragSlot 초기화
 &  : ChangeSlot()          - 슬롯 교체
 *
 */

public class UI_ItemDragSlot : UI_ItemSlot
{
    // 드래그를 시작할 때
    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        // 아이템이 존재할 시 마우스로 들기 가능.
        if (item.IsNull() == true)
            return;

        // dragSlot 활성화
        UI_DragSlot.instance.dragSlotItem = this;
        UI_DragSlot.instance.DragSetImage(icon);

        UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    // 드래그 중일 때
    protected override void OnDragSlot(PointerEventData eventData)
    {
        // 마우스 드래그 방향으로 아이템 이동
        if (item.IsNull() == false && UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    // 드래그가 끝나면
    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // dragSlot 초기화
        UI_DragSlot.instance.SetColor(0);
        UI_DragSlot.instance.dragSlotItem = null;
    }

    // 슬롯 바꾸기
    protected virtual void ChangeSlot(UI_ItemSlot itemSlot) {}
}
