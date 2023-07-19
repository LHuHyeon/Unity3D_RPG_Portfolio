using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
[ 아이템 Slot SubItem 스크립트 ]
1. 모든 아이템 Slot은 UI_ItemSlot을 상속받는다.
2. 아이템 Slot에서 공통적으로 사용되는 기능을 가지고 있다.
*/

/*
TODO
1. 모든 ItemSlot의 icon과 countText를 맞춰주고, UI_ItemSlot에서 enum으로 불러온다.
2. itemCount도 여기서 관리
3. 드래그드랍이 필요한 Slot은 UI_ItemDragSlot.cs 생성 후 구현
4. UI_ItemDragSlot은 Change, Drag Drop PointerEvent 구현
*/

public class UI_ItemSlot : UI_Slot
{
    public ItemData item;

    // 마우스가 슬롯에 닿았다면 정보 활성화
    protected override void OnEnterSlot()
    {
        if (item.IsNull() == false)
        {
            Managers.Game._playScene._slotTip.OnSlotTip(true);
            Managers.Game._playScene._slotTip.background.position = icon.transform.position;
            Managers.Game._playScene._slotTip.RefreshUI(item);
        }
    }

    // 마우스가 슬롯에서 빠져나오면 정보 비활성화
    protected override void OnExitSlot()
    {
        if (item.IsNull() == false)
            Managers.Game._playScene._slotTip.OnSlotTip(false);
    }

    // 아이템 등록
    public virtual void AddItem(ItemData _item, int count = 1)
    {
        item = _item;

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

    public override void ClearSlot()
    {
        base.ClearSlot();
        item = null;
    }
}
