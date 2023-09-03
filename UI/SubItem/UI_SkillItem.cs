using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   UI_SkillItem.cs
 * Desc :   UI_SkillPopup.cs에서 사용되며 스킬을 저장한다.
 *          레벨이 충족 되면 우클릭을 통해 스킬을 활성화할 수 있다.
 *
 & Functions
 &  [Public]
 &  : SetInfo()         - 기능 설정
 &  : ClearSlot()       - 초기화
 &
 &  [Protected]
 &  : OnClickSlot()     - 우클릭하여 스킬 활성화
 &  : OnBeginDragSlot() - 슬롯 드래그 시작
 &  : OnDragSlot()      - 슬롯 드래그 진행
 &  : OnEndDragSlot()   - 슬롯 드래그 끝
 &
 &  [Prviate]
 &  : LevelCheck()      - 스킬 레벨 확인
 *
 */

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

    [SerializeField]
    private int     skillId;

    public override void SetInfo()
    {
        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        // 게임데이터에 스킬 아이디 존재 확인
        if (Managers.Data.Skill.TryGetValue(skillId, out skillData) == false)
            Debug.Log($"SkillData {skillId} : Failed");

        GetText((int)Texts.SkillLevelText).text = skillData.minLevel.ToString();
        icon.sprite = skillData.skillSprite;

        // 시작 시 스킬이 흭득 상태인지 확인
        foreach(SkillData skill in Managers.Game.CurrentSkill)
        {
            // 획득 상태면 Lock 해제
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
        if (skillData.isLock == false)
            base.OnBeginDragSlot(eventData);
    }

    protected override void OnDragSlot(PointerEventData eventData)
    {
        if (skillData.isLock == false)
            base.OnDragSlot(eventData);
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (skillData.isLock == false && skillData.IsNull() == false)
            base.OnEndDragSlot(eventData);
    }

    // 스킬 레벨 체크
    private bool LevelCheck() { return Managers.Game.Level >= skillData.minLevel; }
}
