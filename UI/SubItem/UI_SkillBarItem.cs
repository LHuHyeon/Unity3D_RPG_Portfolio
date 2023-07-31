using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_SkillBarItem : UI_SkillSlot
{
    public Define.KeySkill keySkill;
    public Image coolDownImage;
    public TextMeshProUGUI mpText;

    public override void SetInfo()
    {
        base.SetInfo();

        mpText.text = "";

        // 시작할 때 스킬이 현재 키에 장착 중이라면
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out skillData) == true)
            SetSkill(skillData);
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        if (skillData.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            ClearSlot();
        }

        base.OnEndDragSlot(eventData);
    }

    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

        if (dragSlot.IsNull() == false)
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
    }

    void ChangeSkill(UI_SkillSlot skillSlot)
    {
        SetSkill(skillSlot.skillData);

        // 스킬바에서 온거면 기존 슬롯 삭제
        if (skillSlot is UI_SkillBarItem)
            (skillSlot as UI_SkillBarItem).ClearSlot();
    }

    void SetSkill(SkillData skill)
    {
        // 궁극기 경우 7렙 이상 스킬만 가능
        if (keySkill == Define.KeySkill.R)
        {
            if (skill.minLevel < 7)
                return;
        }

        skillData = skill;

        mpText.text = skillData.skillConsumMp.ToString();

        if (Managers.Game.SkillBarList.ContainsKey(keySkill) == false)
            Managers.Game.SkillBarList.Add(keySkill, skillData);
        else
            Managers.Game.SkillBarList[keySkill] = skillData;

        try
        {
            icon.sprite = skillData.skillSprite;
        }
        catch
        {
            icon.sprite = skillData.skillSprite = Managers.Data.Skill[skillData.skillId].skillSprite;
        }
        
        SetColor(255);
    }

    void Update()
    {
        // 쿨타임
        if (skillData.IsNull() == true)
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

        // 쿨타임 이미지 초기화
        skillData.isCoolDown = false;
        coolDownImage.fillAmount = 1;
        coolDownImage.gameObject.SetActive(false);

        Managers.Game.SkillBarList.Remove(keySkill);
        skillData = null;
    }
}
