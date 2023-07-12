using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillItem : UI_SkillSlot
{
    enum Gameobjects
    {
        LevelBlock,
    }

    enum Texts
    {
        SkillLevelText,
    }

    public int skillId;

    public override void SetInfo()
    {
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        if (Managers.Data.Skill.TryGetValue(skillId, out skillData) == false)
            Debug.Log($"SkillData {skillId} : Failed");

        GetText((int)Texts.SkillLevelText).text = skillData.minLevel.ToString();
        icon.sprite = skillData.skillSprite;

        // 시작 시 스킬이 흭득 상태인지 확인
        foreach(SkillData skill in Managers.Game.CurrentSkill)
        {
            if (skillId == skill.skillId)
            {
                skillData.isLock = skill.isLock;
                break;
            }
        }

        if (skillData.isLock == false)
            Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
        
        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        // 스킬슬롯을 우클릭할 시 레벨체크 후 Lock 해제
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (Input.GetMouseButtonUp(1) && skillData.isLock == true)
            {
                if (LevelCheck() == true)
                {
                    Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
                    {
                        skillData.isLock = false;
                        Managers.Game.CurrentSkill.Add(this.skillData);
                        Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
                    }, Define.SkillOpenMessage);
                }
            }
        }, Define.UIEvent.Click);

        // 스킬이 흭득 상태라면 마우스로 들기 가능.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData.isLock == true || skillData == null)
                return;

            Debug.Log("skillItem BeginDrag");

            UI_DragSlot.instance.dragSlotItem = this;
            UI_DragSlot.instance.DragSetImage(icon);

            UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.BeginDrag);

        // 마우스 드래그 방향으로 이동
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData.isLock == false && skillData != null)
                UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (skillData.isLock == true || skillData == null)
                return;
                
            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);
    }

    // 스킬 레벨 체크
    bool LevelCheck()
    {
        if (Managers.Game.Level >= skillData.minLevel)
        {
            return true;
        }

        return false;
    }
}
