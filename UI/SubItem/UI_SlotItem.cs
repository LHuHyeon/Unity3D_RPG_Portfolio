using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    1. Slot의 기본 코드를 여기서 관리
    2. 모든 Slot은 이 클래스를 상속 받음.
*/

public class UI_SlotItem : UI_Base
{
    public Define.SlotType slotType = Define.SlotType.Unknown;

    public ItemData item;
    public Image icon;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetInfo();

        return true;
    }

    public virtual void SetInfo()
    {
        SetEventHandler();
    }

    protected virtual void SetEventHandler()
    {
        if (slotType == Define.SlotType.Skill)
            return;
            
        if (slotType == Define.SlotType.Inven || slotType == Define.SlotType.Equipment)
        {
            gameObject.BindEvent((PointerEventData eventData)=>
            {
                if (item != null)
                {
                    Managers.Game._playScene._slotTip.OnSlotTip(true);

                    Managers.Game._playScene._slotTip.background.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    Managers.Game._playScene._slotTip.RefreshUI(item);
                }
            }, Define.UIEvent.Enter);

            gameObject.BindEvent((PointerEventData eventData)=>
            {
                if (item != null)
                    Managers.Game._playScene._slotTip.OnSlotTip(false);
            }, Define.UIEvent.Exit);
        }
        
        // 아이템이 존재할 시 마우스로 들기 가능.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item == null)
                return;

            // 인벤 이라면 Lock 확인
            if (this is UI_InvenItem)
            {
                if ((this as UI_InvenItem).IsLock == true)
                    return;
            }

            UI_DragSlot.instance.dragSlotItem = this;
            UI_DragSlot.instance.DragSetImage(icon);

            UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.BeginDrag);

        // 마우스 드래그 방향으로 아이템 이동
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null && UI_DragSlot.instance.dragSlotItem != null)
                UI_DragSlot.instance.icon.transform.position = eventData.position;
        }, Define.UIEvent.Drag);
    }

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }

    // 아이템 등록
    public virtual void AddItem(ItemData _item, int count = 1)
    {
        item = _item;
        icon.sprite = item.itemIcon;

        // 색 활성화
        SetColor(255);
    }

    // 슬롯 초기화
    public virtual void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        
        SetColor(0);
    }

    void RefreshUI()
    {

    }
}
