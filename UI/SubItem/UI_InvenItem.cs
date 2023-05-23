using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InvenItem : UI_Base
{
    public ItemData item;
    public Image itemImage;
    public TextMeshProUGUI itemCountText;
    public int itemCount;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SetInfo();

        return true;
    }

    public void SetInfo()
    {
        SetEventHandler();
    }

    void SetEventHandler()
    {
        // 아이템이 존재할 시 마우스로 들기 가능.
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item == null)
                return;

            UI_DragSlot.instance.dragInvenSlot = this;
            UI_DragSlot.instance.DragSetImage(itemImage);

            UI_DragSlot.instance.itemImage.transform.position = eventData.position;
        }, Define.UIEvent.BeginDrag);

        // 마우스 드래그 방향으로 아이템 이동
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item != null)
                UI_DragSlot.instance.itemImage.transform.position = eventData.position;
        }, Define.UIEvent.Drag);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // 아이템 버리기
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragInvenSlot = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 아이템 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_InvenItem dragSlot = UI_DragSlot.instance.dragInvenSlot;
            if (dragSlot == this || dragSlot.item.id == item.id)
                return;

            if (dragSlot != null)
            {
                // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                if (item == dragSlot.item)
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
        UI_InvenItem dragSlot = UI_DragSlot.instance.dragInvenSlot;
        AddItem(dragSlot.item, dragSlot.itemCount);

        // 현재 슬롯 아이템을 드래그된 슬롯에 Add
        if (_tempItem != null)
            UI_DragSlot.instance.dragInvenSlot.AddItem(_tempItem, _tempItemCount);
        else
            UI_DragSlot.instance.dragInvenSlot.ClearSlot(false);
    }

    // 투명도 설정 (0 ~ 255)
    void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
        itemCountText.color = color;
    }

    // 아이템 등록
    public void AddItem(ItemData _item, int count = 1)
    {
        item = _item;
        itemImage.sprite = item.itemIcon;

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

        // 색 활성화
        SetColor(255);
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
    public void ClearSlot(bool isRemove=true)
    {
        item = null;
        itemImage.sprite = null;
        itemCount = 0;
        itemCountText.text = "0";
        
        SetColor(0);
    }

    void RefreshUI()
    {

    }
}
