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

    // 아이템을 팔때 사용 변수
    public GameObject _lock;    // 판매 등록될 시 인벤 Lock

    private bool isLock = false;
    public bool IsLock
    {
        get { return isLock; }
        set {
            isLock = value;
            _lock.SetActive(isLock);
        }
    }

    public override void SetInfo()
    {
        slotType = Define.SlotType.Inven;
        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item == null || UI_DragSlot.instance.dragSlotItem != null)
                return;

            // 장비 장착 or 아이템 사용
            if (Input.GetMouseButtonUp(1))
            {
                if ((item is EquipmentData) == true)
                {
                    Managers.Game._playScene._equipment.SetEquipment(this);
                }
                else if ((item is UseItemData) == true)
                {
                    // 아이템 사용이 성공적으로 됐다면 -1 차감
                    if ((item as UseItemData).UseItem(this.item) == true)
                        SetCount(-1);
                }
            }
        }, Define.UIEvent.Click);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (UI_DragSlot.instance.dragSlotItem == null)
                return;

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
            if (UI_DragSlot.instance.dragSlotItem == null)
                return;
                
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            // 강화창에서 온거면
            if (dragSlot is UI_UpgradeItem)
                AddSlot<UI_UpgradeItem>(dragSlot as UI_UpgradeItem);

            // 장비창에서 온거면
            if (dragSlot is UI_ArmorItem || dragSlot is UI_WeaponItem)
            {
                if (dragSlot is UI_ArmorItem)
                    AddSlot<UI_ArmorItem>(dragSlot as UI_ArmorItem);
                else if (dragSlot is UI_WeaponItem)
                    AddSlot<UI_WeaponItem>(dragSlot as UI_WeaponItem);
            }

            // 소비 아이템 슬롯에서 온거면 (Scene 슬롯)
            if (dragSlot is UI_UseItemSlot)
            {
                UI_UseItemSlot useItemSlot = dragSlot as UI_UseItemSlot;
                AddSlot<UI_UseItemSlot>(useItemSlot, useItemSlot.itemCount);
            }

            // 인벤토리에서 온거면
            if (dragSlot is UI_InvenItem)
            {
                UI_InvenItem invenSlot = dragSlot as UI_InvenItem;

                // 두 슬롯의 아이템이 같은 아이템일 경우 개수 체크
                if (item == invenSlot.item && (invenSlot.item is UseItemData))
                {
                    int addValue = itemCount + invenSlot.itemCount;
                    if (addValue > item.itemMaxCount)
                    {
                        invenSlot.SetCount(-(item.itemMaxCount-itemCount));
                        SetCount(item.itemMaxCount - itemCount);
                    }
                    else
                    {
                        SetCount(invenSlot.itemCount);
                        invenSlot.ClearSlot();  // 들고 있었던 슬롯은 초기화
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

    // 슬롯 받기
    private void AddSlot<T>(T slot, int count = 1) where T : UI_SlotItem
    {
        // 아이템이 있다면 다른 슬롯 || 없다면 지금 슬롯에 넣기
        if (item != null)
            Managers.Game._playScene._inventory.AcquireItem(slot.item, count);
        else
            AddItem(slot.item, count);

        slot.ClearSlot();
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float _alpha)
    {
        base.SetColor(_alpha);

        itemCountText.color = icon.color;
    }

    // 아이템 등록
    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        // 장비가 아니라면 개수 설정
        if ((item is UseItemData) == true)
        {
            (item as UseItemData).itemCount = count;
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
        
        if (item is UseItemData)
            (item as UseItemData).itemCount += count;

        // 개수가 없다면
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화
    public override void ClearSlot()
    {
        base.ClearSlot();

        IsLock = false;
        itemCount = 0;
        itemCountText.text = "0";
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        
        SetColor(0);
    }

    void RefreshUI()
    {

    }
}
