using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_SkillSlot : UI_SlotItem
{
    public SkillData skillData;

    public override void SetInfo()
    {
        slotType = Define.SlotType.Skill;
        base.SetInfo();
    }
}
