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

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1) && skillData.isLock == true)
        {
            if (LevelCheck() == true)
            {
                UI_ConfirmPopup confirmPopup = Managers.UI.ShowPopupUI<UI_ConfirmPopup>();
                if (confirmPopup.IsNull() == true) return;
                
                confirmPopup.SetInfo(()=>
                {
                    skillData.isLock = false;
                    Managers.Game.CurrentSkill.Add(this.skillData);
                    Managers.Resource.Destroy(GetObject((int)Gameobjects.LevelBlock));
                }, Define.SkillOpenMessage);
            }
            else
                Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
        }
    }

    protected override void OnBeginDragSlot(PointerEventData eventData)
    {
        if (skillData.isLock == false) base.OnBeginDragSlot(eventData);
    }

    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (skillData.isLock == false) base.OnDragSlot(eventData);
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (skillData.isLock == false && skillData.IsNull() == false)
            base.OnEndDragSlot(eventData);
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
