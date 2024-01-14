using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
 * File :   UI_SkillBarSlot.cs
 * Desc :   Scene UI의 하단 퀵슬롯에서 스킬바로 사용되며
 *          스킬이 적용될 시 key를 눌러 스킬 사용이 가능하다.
 *
 & Functions
 &  [Public]
 &  : SetInfo()         - 기능 설정
 &  : ClearSlot()       - 초기화
 &
 &  [Protected]
 &  : OnEndDragSlot()   - 마우스 클릭을 해제하면 "UI가 아니면 초기화"
 &  : OnDropSlot()      - 현재 슬롯에 마우스 클릭을 때면 "스킬 등록"
 &
 &  [Prviate]
 &  : ChangeSkill()     - 스킬 교체
 &  : SetSkill()        - 스킬 설정
 &  : UpdateCoolDown()  - 쿨타임
 &  : IsCoolDown()      - 쿨타임 여부
 *
 */

public class UI_SkillBarSlot : UI_SkillSlot
{
    enum Images
    {
        CoolDownBlock,
        ItemImage,
    }

    enum Texts
    {
        MpText,
    }

    [SerializeField]
    private Define.KeySkill     keySkill;       // 입력 key
    private Image               coolDownImage;  // 쿨타임 이미지

    public override void SetInfo()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        coolDownImage = GetImage((int)Images.CoolDownBlock);
        coolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        SetEventHandler();

        // 시작할 때 스킬이 현재 키에 장착 중이라면
        if (Managers.Game.SkillBarList.TryGetValue(keySkill, out skillData) == true)
        {
            skillData.skillSprite = Managers.Data.Skill[skillData.skillId].skillSprite;
            SetSkill(skillData);
        }
    }

    void Update()
    {
        UpdateCoolDown();
    }

    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 마우스 마지막 드래그 위치가 UI가 아니라면
        if (skillData.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 현재 전투 중인 몬스터가 없다면 초기화
            if (Managers.Game.currentMonster.IsNull() == true)
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

    private void ChangeSkill(UI_SkillSlot skillSlot)
    {
        // 스킬 설정
        SetSkill(skillSlot.skillData);

        // 넘어온 스킬의 쿨타임 여부
        IsCoolDown(skillData.isCoolDown);

        // 기존 슬롯 삭제
        if (skillSlot is UI_SkillBarSlot)
            (skillSlot as UI_SkillBarSlot).ClearSlot();
    }

    private void SetSkill(SkillData skill)
    {
        // 궁극기 경우 5렙 이상 스킬만 가능
        if (keySkill == Define.KeySkill.R)
        {
            if (skill.minLevel < 5)
                return;
        }
        
        // 기존 스킬 쿨타임 여부
        IsCoolDown(skillData.isCoolDown);

        skillData = skill;

        GetText((int)Texts.MpText).text = skillData.skillConsumMp.ToString();

        // 게임 데이터에 스킬 저장
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

    // 쿨타임 진행
    private void UpdateCoolDown()
    {
        // 쿨타임
        if (skillData.IsNull() == true)
            return;
        
        if (skillData.isCoolDown == true)
        {
            // 쿨타임 객체 활성화
            if (coolDownImage.gameObject.activeSelf == false)
                coolDownImage.gameObject.SetActive(true);

            // 시계 방향으로 밝아지는 fillAmount
            coolDownImage.fillAmount -= 1 * Time.smoothDeltaTime / skillData.skillCoolDown;
            
            // fillAmount가 0이 되면 쿨타임 끝
            if (coolDownImage.fillAmount <= 0)
            {
                skillData.isCoolDown = false;
                coolDownImage.fillAmount = 1;
                coolDownImage.gameObject.SetActive(false);
            }
        }
    }

    // 쿨타임 여부
    private void IsCoolDown(bool isTrue)
    {
        coolDownImage.fillAmount = 1;

        skillData.isCoolDown = isTrue;
        coolDownImage.gameObject.SetActive(isTrue);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        // 쿨타임 이미지 초기화
        coolDownImage.fillAmount = 1;
        coolDownImage.gameObject.SetActive(false);

        GetText((int)Texts.MpText).text = "";

        Managers.Game.SkillBarList.Remove(keySkill);
        skillData = null;
    }
}
