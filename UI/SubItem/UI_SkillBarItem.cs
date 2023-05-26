using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillBarItem : UI_SkillSlot
{
    public Define.KeySkill keySkill;

    public override void SetInfo()
    {
        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        // 스킬이 등록된 상태라면 마우스로 들기 가능.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData == null)
                return;

            UI_DragSlot.instance.dragSlotItem = this;
            UI_DragSlot.instance.DragSetImage(icon);

            UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.BeginDrag);

        // 마우스 드래그 방향으로 이동
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData != null)
                UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData != null && !EventSystem.current.IsPointerOverGameObject())
            {
                ClearSlot();
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 스킬받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            if (dragSlot != null)
            {
                // 자기 자신이라면
                if (dragSlot == this)
                    return;

                // 스킬 슬롯 확인
                if ((dragSlot is UI_SkillSlot) == false)
                    return;

                // 스킬 장착
                SetSkill(dragSlot as UI_SkillSlot);
            }

        }, Define.UIEvent.Drop);
    }

    void SetSkill(UI_SkillSlot skillSlot)
    {
        skillData = skillSlot.skillData;

        // 스킬바에서 온거면 기존 슬롯 삭제
        if (skillSlot is UI_SkillBarItem)
            skillSlot.ClearSlot();

        if (Managers.Game.SkillBarList.ContainsKey(keySkill) == false)
            Managers.Game.SkillBarList.Add(keySkill, skillData);
        else
            Managers.Game.SkillBarList[keySkill] = skillData;

        icon.sprite = skillData.skillSprite;
        SetColor(255);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.SkillBarList.Remove(keySkill);
        skillData = null;
    }
}
