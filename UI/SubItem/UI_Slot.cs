using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
[ Slot SubItem 스크립트 ]
1. 모든 Slot의 부모이다.
*/

/*
1. 아이템 관련 슬롯 (아이템 정보 활성화, 옮기기)
2. 스킬 관련 슬롯 (옮기기)
3. 정보만 표시될 슬롯 (아이템 정보 활성화)
*/

public abstract class UI_Slot : UI_Base
{
    enum Images { ItemImage, }
    public Image icon;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetInfo();
        SetEventHandler();

        return true;
    }

    public virtual void SetInfo()
    {
        BindImage(typeof(Images));
        icon = GetImage((int)Images.ItemImage);
    }

    public virtual void RefreshUI() {}

    protected virtual void SetEventHandler()
    {
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEnterSlot(eventData); }, Define.UIEvent.Enter);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnExitSlot(eventData); }, Define.UIEvent.Exit);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnClickSlot(eventData); }, Define.UIEvent.Click);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnBeginDragSlot(eventData); }, Define.UIEvent.BeginDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDragSlot(eventData); }, Define.UIEvent.Drag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnEndDragSlot(eventData); }, Define.UIEvent.EndDrag);
        gameObject.BindEvent((PointerEventData eventData)=>{ OnDropSlot(eventData); }, Define.UIEvent.Drop);
    }
    
    protected virtual void OnEnterSlot(PointerEventData eventData) {}
    protected virtual void OnExitSlot(PointerEventData eventData) {}
    protected virtual void OnClickSlot(PointerEventData eventData) {}
    protected virtual void OnBeginDragSlot(PointerEventData eventData) {}
    protected virtual void OnDragSlot(PointerEventData eventData) {}
    protected virtual void OnEndDragSlot(PointerEventData eventData) {}
    protected virtual void OnDropSlot(PointerEventData eventData) {}

    // 투명도 설정 (0 ~ 255)
    protected virtual void SetColor(float _alpha)
    {
        Color color = icon.color;
        color.a = _alpha;
        icon.color = color;
    }

    // 슬롯 초기화
    public virtual void ClearSlot()
    {
        icon.sprite = null;
        
        SetColor(0);
    }
}
