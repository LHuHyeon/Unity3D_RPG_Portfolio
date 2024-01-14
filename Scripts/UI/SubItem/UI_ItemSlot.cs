using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_ItemSlot.cs
 * Desc :   모든 Item관련 Slot은 해당 클래스를 상속받는다.
 *
 & Functions
 &  [Public]
 &  : SetInfo()         - 기능 설정
 &  : AddItem()         - 아이템 추가
 &  : SetCount()        - 개수 설정
 &  : ClearSlot()       - 초기화
 &
 &  [Protected]
 &  : OnEnterSlot()     - 마우스가 Slot과 접촉하면 SlotTip 활성화
 &  : OnExitSlot()      - 마우스가 Slot과 접촉이 해제되면 SlotTip 비활성화
 &  : SetColor()        - 색깔 설정
 *
 */

public class UI_ItemSlot : UI_Slot
{
    enum Texts { ItemCountText, }

    public ItemData     item;
    public int          itemCount;

    public override void SetInfo()
    {
        base.SetInfo();

        BindText(typeof(Texts));
    }

    // 아이템 등록
    public virtual void AddItem(ItemData _item, int count = 1)
    {
        item = _item;

        // 장비가 아니라면 개수 설정
        if ((item is UseItemData) == true)
        {
            (item as UseItemData).itemCount = count;
            itemCount = count;
            GetText((int)Texts.ItemCountText).text = itemCount.ToString();
        }
        else
        {
            itemCount = 1;

            if (GetText((int)Texts.ItemCountText).IsNull() == false)
                GetText((int)Texts.ItemCountText).text = "";
        }

        if (item.itemIcon.IsFakeNull() == true)
            item.itemIcon = Managers.Data.Item[item.id].itemIcon;

        // Spirte 넣기
        // try는 null체크 시 없는 객체면 item Data에서 빼옴.
        try
        {
            icon.sprite = item.itemIcon;
        }
        catch 
        {
            icon.sprite = item.itemIcon = Managers.Data.Item[item.id].itemIcon;
        }

        // 색 활성화
        SetColor(255);
    }

    // 아이템 개수 업데이트
    public virtual void SetCount(int count = 1)
    {
        itemCount += count;
        GetText((int)Texts.ItemCountText).text = itemCount.ToString();
        
        if (item is UseItemData)
            (item as UseItemData).itemCount += count;

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    // 마우스가 슬롯에 닿았다면 정보 활성화
    protected override void OnEnterSlot(PointerEventData eventData)
    {
        if (item.IsNull() == false)
        {
            Managers.Game._playScene._slotTip.OnSlotTip(true);
            Managers.Game._playScene._slotTip.background.position = icon.transform.position;
            Managers.Game._playScene._slotTip.RefreshUI(item);
        }
    }

    // 마우스가 슬롯에서 빠져나오면 정보 비활성화
    protected override void OnExitSlot(PointerEventData eventData)
    {
        if (item.IsNull() == false)
            Managers.Game._playScene._slotTip.OnSlotTip(false);
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float _alpha)
    {
        base.SetColor(_alpha);

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).color = icon.color;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        item = null;
        itemCount = 0;

        if (GetText((int)Texts.ItemCountText).IsNull() == false)
            GetText((int)Texts.ItemCountText).text = "";

        Managers.Game._playScene._slotTip.OnSlotTip(false);
    }
}
