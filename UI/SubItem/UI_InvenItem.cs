using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InvenItem : UI_SlotItem
{
    public TextMeshProUGUI itemCountText;
    public int itemCount;

    public override void SetInfo()
    {
        slotType = Define.SlotType.Inven;
        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // 아이템 버리기
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_InvenItem dragSlot = UI_DragSlot.instance.dragSlotItem as UI_InvenItem;

            if (dragSlot != null)
            {
                // 자기 자신이라면
                if (dragSlot == this)
                    return;

                // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                if (item == dragSlot.item && (dragSlot.item is UseItemData))
                {
                    int addValue = itemCount + dragSlot.itemCount;
                    if (addValue > item.itemMaxCount)
                    {
                        dragSlot.SetCount(-(item.itemMaxCount-itemCount));
                        SetCount(item.itemMaxCount - itemCount);
                    }
                    else
                    {
                        SetCount(dragSlot.itemCount);
                        dragSlot.ClearSlot();  // 들고 있었던 슬롯은 초기화
                    }
                }
                else
                    ChangeSlot();
            }

        }, Define.UIEvent.Drop);
    }

    // 현재 슬롯을 다른 슬롯과 바꿀 때 사용하는 메소드
    private void ChangeSlot()
    {
        ItemData _tempItem = item;
        int _tempItemCount = itemCount;

        // 드래그된 슬롯을 현재 슬롯에 Add
        UI_InvenItem dragSlot = UI_DragSlot.instance.dragSlotItem as UI_InvenItem;
        AddItem(dragSlot.item, dragSlot.itemCount);

        // 현재 슬롯 아이템을 드래그된 슬롯에 Add
        if (_tempItem != null)
            UI_DragSlot.instance.dragSlotItem.AddItem(_tempItem, _tempItemCount);
        else
            UI_DragSlot.instance.dragSlotItem.ClearSlot();
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float _alpha)
    {
        base.SetColor(_alpha);

        itemCountText.color = itemImage.color;
    }

    // 아이템 등록
    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        // 장비가 아니라면 개수 설정
        if ((item is UseItemData) == true)
        {
            itemCount = count;
            itemCountText.text = itemCount.ToString();
        }
        else
        {
            itemCount = 0;
            itemCountText.text = "";
        }
    }

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        itemCountText.text = itemCount.ToString();

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        base.ClearSlot();

        itemCount = 0;
        itemCountText.text = "0";
        
        SetColor(0);
    }

    void RefreshUI()
    {

    }
}
