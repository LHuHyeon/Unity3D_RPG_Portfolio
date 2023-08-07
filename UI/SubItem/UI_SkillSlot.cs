using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 스킬 Slot 스크립트 ]
1. 스킬 Slot의 부모 스크립트다.
*/

public class UI_SkillSlot : UI_Slot
{
    public SkillData skillData;

    public override void SetInfo()
    {
        base.SetInfo();
    }

    // 스킬이 등록된 상태라면 마우스로 들기 가능.
    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        if (skillData.IsNull() == true)
            return;

        UI_DragSlot.instance.dragSlotItem = this;
        UI_DragSlot.instance.DragSetImage(icon);

        UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    // 마우스 드래그 방향으로 이동
    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (skillData.IsNull() == false)
            UI_DragSlot.instance.icon.transform.position = eventData.position;
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        UI_DragSlot.instance.SetColor(0);
        UI_DragSlot.instance.dragSlotItem = null;
    }
}
