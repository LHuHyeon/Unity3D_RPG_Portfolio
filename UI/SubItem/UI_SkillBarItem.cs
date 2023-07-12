using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_SkillBarItem : UI_SkillSlot
{
    public Define.KeySkill keySkill;
    public Image coolDownImage;

    public override void SetInfo()
    {
        base.SetInfo();

        // 시작할 때 스킬이 현재 키에 장착 중이라면
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out skillData) == true)
            SetSkill(skillData);
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
                ChangeSkill(dragSlot as UI_SkillSlot);
            }

        }, Define.UIEvent.Drop);
    }

    void ChangeSkill(UI_SkillSlot skillSlot)
    {
        // 스킬바에서 온거면 기존 슬롯 삭제
        if (skillSlot is UI_SkillBarItem)
            skillSlot.ClearSlot();

        SetSkill(skillSlot.skillData);
    }

    void SetSkill(SkillData skill)
    {
        // 궁극기 경우 10렙 이상 스킬만 가능
        if (keySkill == Define.KeySkill.R)
        {
            if (skill.minLevel < 10)
                return;
        }

        skillData = skill;

        if (Managers.Game.SkillBarList.ContainsKey(keySkill) == false)
            Managers.Game.SkillBarList.Add(keySkill, skillData);
        else
            Managers.Game.SkillBarList[keySkill] = skillData;

        icon.sprite = skillData.skillSprite;
        SetColor(255);
    }

    void Update()
    {
        // 쿨타임
        if (skillData == null)
            return;
        
        if (skillData.isCoolDown == true)
        {
            if (coolDownImage.gameObject.activeSelf == false)
                coolDownImage.gameObject.SetActive(true);

            coolDownImage.fillAmount -= 1 * Time.smoothDeltaTime / skillData.skillCoolDown;
            
            if (coolDownImage.fillAmount <= 0)
            {
                skillData.isCoolDown = false;
                coolDownImage.fillAmount = 1;
                coolDownImage.gameObject.SetActive(false);
            }
        }
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.SkillBarList.Remove(keySkill);
        skillData = null;
    }
}
