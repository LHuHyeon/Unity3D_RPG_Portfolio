using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_WeaponItem : UI_SlotItem
{
    public Define.WeaponType weaponType = Define.WeaponType.Unknown;
    public WeaponItemData weaponItem;

    public override void SetInfo()
    {
        slotType = Define.SlotType.Equipment;
        Managers.Game._playScene._equipment.weaponSlot = this;

        base.SetInfo();
    }

    protected override void SetEventHandler()
    {
        base.SetEventHandler();

        gameObject.BindEvent((PointerEventData eventData)=>
        {
            if (item == null || UI_DragSlot.instance.dragSlotItem != null)
                return;

            // 장비 벗기
            if (Input.GetMouseButtonUp(1))
            {
                Managers.Game._playScene._inventory.AcquireItem(weaponItem);
                ClearSlot();
            }
        }, Define.UIEvent.Click);

        // 드래그가 끝났을 때
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            // 아이템을 버린 위치가 UI가 아니라면
            if (item != null && !EventSystem.current.IsPointerOverGameObject())
            {
                // 아이템 인벤으로 이동
            }

            UI_DragSlot.instance.SetColor(0);
            UI_DragSlot.instance.dragSlotItem = null;

        }, Define.UIEvent.EndDrag);

        // 이 슬롯에 마우스 클릭이 끝나면 장비 받기
        gameObject.BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            if (dragSlot != null)
            {
                // 자기 자신이라면
                if (dragSlot == this)
                    return;

                // 장비 장착 (or 교체)
                ChangeWeapon(dragSlot);
            }

        }, Define.UIEvent.Drop);
    }

    // 무기 장착
    public void ChangeWeapon(UI_SlotItem itemSlot)
    {
        // 장비 확인
        if ((itemSlot.item is WeaponItemData) == false)
            return;

        // 같은 부위 확인
        WeaponItemData weapon = itemSlot.item as WeaponItemData;
        if (weaponType != weapon.weaponType)
            return;

        // 레벨 체크
        if (Managers.Game.Level < weapon.minLevel)
            return;

        ItemData _tempItem = item;

        // 장비 장착
        AddItem(itemSlot.item);

        // 기존 장비 인벤 이동
        UI_InvenItem inven = itemSlot as UI_InvenItem;
        if (_tempItem != null)
            inven.AddItem(_tempItem);
        else
            inven.ClearSlot();
    }

    public override void AddItem(ItemData _item, int count = 1)
    {
        base.AddItem(_item, count);

        weaponItem = _item as WeaponItemData;
        
        // 장착 중인 무기가 있다면 비활성화
        if (Managers.Game.CurrentWeapon != null)
        {
            Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        }

        Managers.Game.CurrentWeapon = weaponItem;

        weaponItem.charEquipment.SetActive(true);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        Managers.Game.CurrentWeapon.charEquipment.SetActive(false);
        Managers.Game.CurrentWeapon = null;
        weaponItem = null;

        Managers.Game._playScene._slotTip.OnSlotTip(false);
    }
}
