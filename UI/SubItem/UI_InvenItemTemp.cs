using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
TODO : Unity 오브젝트와 enum 이름 맞춰주기
*/

public class UI_InvenItemTemp : UI_ItemSlot
{
    enum GameObjects { Lock, }
    enum Images { ItemImage, }
    enum Texts { ItemCountText, }

    public int invenNumber; // 인벤 자리 번호
    public int itemCount;

    public GameObject _lock;    // 상점 판매 등록될 시 인벤 Lock

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
        base.SetInfo();

        BindObject(typeof(GameObjects));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
    }

    // 슬롯 우클릭
    protected override void OnClickSlot()
    {
        if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            return;

        // 슬롯 우클릭
        // if (Input.GetMouseButtonUp(1))
        // {
        //     // 상호작용 중이라면
        //     if (Managers.Game.IsInteract == true)
        //     {
        //         Managers.Game.GetSlotInteract(this);
        //         return;
        //     }

        //     // 장비 or 소비 아이템이라면
        //     if ((item is EquipmentData) == true)
        //     {
        //         // 장착 레벨 확인
        //         if (Managers.Game.Level >= (item as EquipmentData).minLevel)
        //             Managers.Game._playScene._equipment.SetEquipment(this);
        //         else
        //             Managers.UI.ShowPopupUI<UI_GuidePopup>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
        //     }
        //     else if ((item is UseItemData) == true)
        //     {
        //         // 아이템 사용이 성공적으로 됐다면 -1 차감
        //         if ((item as UseItemData).UseItem(this.item) == true)
        //             SetCount(-1);
        //     }
        // }
    }

    protected override void OnEndDragSlot()
    {
        if (UI_DragSlot.instance.dragSlotItem.IsNull() == true)
            return;

        // 아이템을 버린 위치가 UI가 아니라면
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 아이템 버리기
        }

        UI_DragSlot.instance.SetColor(0);
        UI_DragSlot.instance.dragSlotItem = null;
    }

    protected override void OnDropSlot()
    {
        if (UI_DragSlot.instance.dragSlotItem.IsNull() == true)
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
        if (_tempItem.IsNull() == false)
            UI_DragSlot.instance.dragSlotItem.AddItem(_tempItem, _tempItemCount);
        else
            UI_DragSlot.instance.dragSlotItem.ClearSlot();
    }

    // 슬롯 받기
    private void AddSlot<T>(T slot, int count = 1) where T : UI_SlotItem
    {
        // 아이템이 있다면 다른 슬롯 || 없다면 지금 슬롯에 넣기
        if (item.IsNull() == false)
            Managers.Game._playScene._inventory.AcquireItem(slot.item, count);
        else
            AddItem(slot.item, count);

        slot.ClearSlot();
    }

    // 투명도 설정 (0 ~ 255)
    protected override void SetColor(float _alpha)
    {
        base.SetColor(_alpha);

        GetText((int)Texts.ItemCountText).color = icon.color;
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
            GetText((int)Texts.ItemCountText).text = itemCount.ToString();
        }
        else
        {
            itemCount = 0;
            GetText((int)Texts.ItemCountText).text = "";
        }

        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(invenNumber) == false)
            Managers.Game.InvenItem.Add(invenNumber, _item);
        else
            Managers.Game.InvenItem[invenNumber] = _item;
    }

    // 아이템 개수 업데이트
    public void SetCount(int count = 1)
    {
        itemCount += count;
        GetText((int)Texts.ItemCountText).text = itemCount.ToString();
        
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
        GetText((int)Texts.ItemCountText).text = "0";
        Managers.Game._playScene._slotTip.OnSlotTip(false);

        // 매니저에 저장
        if (Managers.Game.InvenItem.ContainsKey(invenNumber) == true)
            Managers.Game.InvenItem[invenNumber] = null;
        
        SetColor(0);
    }
}
