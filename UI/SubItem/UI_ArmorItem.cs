using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ArmorItem : UI_SlotItem
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;
    public ArmorItemData armorItem;

    public override void SetInfo()
    {
        slotType = Define.SlotType.Equipment;
        Managers.Game._playScene._equipment.armorSlots.Add(this);

        // 해당 부위 장비가 장착되어 있다면
        if (Managers.Game.CurrentArmor.TryGetValue(armorType, out armorItem) == true)
            Code_AddArmor(armorItem);

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
                Managers.Game._playScene._inventory.AcquireItem(armorItem);
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
                ChangeArmor(dragSlot);
            }

        }, Define.UIEvent.Drop);
    }

    public void ChangeArmor(UI_SlotItem itemSlot)
    {
        // 장비 확인
        if ((itemSlot.item is ArmorItemData) == false)
            return;

        // 같은 부위 확인
        ArmorItemData armor = itemSlot.item as ArmorItemData;
        if (armorType != armor.armorType)
            return;

        // 레벨 체크
        if (Managers.Game.Level < armor.minLevel)
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

        armorItem = _item as ArmorItemData;
        
        // 장착 중인 장비가 있다면 비활성화
        if (Managers.Game.CurrentArmor.ContainsKey(armorType) == true)
        {
            // 현재 장착한 장비 가져오기
            ArmorItemData currentArmor = Managers.Game.CurrentArmor[armorType];

            // 플레이어가 현재 입고 있는 장비 오브젝트 비활성화
            EquipmentActive(currentArmor, false);

            // 스탯 해제
            Managers.Game.RefreshArmor(currentArmor, false);
        }

        // 장비 장착 진행
        if (Managers.Game.CurrentArmor.ContainsKey(armorType) == false)
            Managers.Game.CurrentArmor.Add(armorType, armorItem);
        else
            Managers.Game.CurrentArmor[armorType] = armorItem;

        // 장비 오브젝트 활성화
        EquipmentActive(armorItem, true);

        // 스탯 적용
        Managers.Game.RefreshArmor(armorItem, true);
    }

    // 코드로 장비 장착
    public void Code_AddArmor(ItemData _item)
    {
        base.AddItem(_item);

        armorItem = _item as ArmorItemData;

        // 장비 오브젝트 활성화
        EquipmentActive(armorItem, true);

        // 스탯 적용
        Managers.Game.RefreshArmor(armorItem, true);
    }

    // 캐릭터의 보여지는 장비 오브젝트 활성화 여부
    void EquipmentActive(ArmorItemData armor, bool isActive)
    {
        // 아이템이 현재 입고 있는 장비를 알고 있다면
        if (armor.charEquipment != null)
        {
            foreach(GameObject obj in armor.charEquipment)
                obj.SetActive(isActive);

            return;
        }

        // 모른다면 id로 찾기
        PlayerController player = Managers.Game.GetPlayer().GetComponent<PlayerController>();

        List<GameObject> objList = new List<GameObject>();
        if (player.charEquipment.TryGetValue(armor.id, out objList) == false)
        {
            Debug.Log($"{armor.id} : 활성화 실패");
            return;
        }

        // 아이템 안에 넣어주기
        armor.charEquipment = objList;

        foreach(GameObject obj in objList)
            obj.SetActive(isActive);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        EquipmentActive(armorItem, false);              // 장비 비활성화
        Managers.Game.RefreshArmor(armorItem, false);   // 장비 스탯 해제
        armorItem = null;

        Managers.Game._playScene._slotTip.OnSlotTip(false);
    }
}
