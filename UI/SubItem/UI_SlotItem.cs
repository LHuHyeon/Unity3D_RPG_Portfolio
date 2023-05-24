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
    public Image itemImage;

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
        // 아이템이 존재할 시 마우스로 들기 가능.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item == null)
                return;

            UI_DragSlot.instance.dragSlotItem = this;
            UI_DragSlot.instance.DragSetImage(itemImage);

            UI_DragSlot.instance.itemImage.transform.position = eventData.position;
        }, Define.UIEvent.BeginDrag);

        // 마우스 드래그 방향으로 아이템 이동
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
                UI_DragSlot.instance.itemImage.transform.position = eventData.position;
        }, Define.UIEvent.Drag);
    }

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 등록
    public virtual void AddItem(ItemData _item, int count = 1)
    {
        item = _item;
        itemImage.sprite = item.itemIcon;

        // 색 활성화
        SetColor(255);
    }

    // 슬롯 초기화
    public virtual void ClearSlot()
    {
        item = null;
        itemImage.sprite = null;
        
        SetColor(0);
    }

    void RefreshUI()
    {

    }
}
