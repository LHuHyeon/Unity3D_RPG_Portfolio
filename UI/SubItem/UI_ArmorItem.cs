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

                // 장비 확인
                if ((dragSlot.item is ArmorItemData) == false)
                    return;

                // 같은 부위 확인
                ArmorItemData armor = dragSlot.item as ArmorItemData;
                if (armorType != armor.armorType)
                    return;

                // 장비 장착 (or 교체)
                ChangeArmor();
            }

        }, Define.UIEvent.Drop);
    }

    private void ChangeArmor()
    {
        ItemData _tempItem = item;

        // 장비 장착
        // UI_ArmorItem dragSlot = UI_DragSlot.instance.dragSlotItem as UI_ArmorItem;
        AddItem(UI_DragSlot.instance.dragSlotItem.item);

        // 기존 장비 인벤 이동
        UI_InvenItem inven = UI_DragSlot.instance.dragSlotItem as UI_InvenItem;
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
            EquipmentActive(currentArmor.id, false);
        }

        // 장비 장착 진행
        if (Managers.Game.CurrentArmor.ContainsKey(armorType) == false)
            Managers.Game.CurrentArmor.Add(armorType, armorItem);
        else
            Managers.Game.CurrentArmor[armorType] = armorItem;

        // 장비 오브젝트 활성화
        EquipmentActive(armorItem.id, true);
    }

    // 캐릭터의 보여지는 장비 오브젝트 활성화 여부
    void EquipmentActive(int armorId, bool isActive)
    {   
        // TODO : KEy 확인 후 진행하기
        // TODO : UI 누르는 과정에서 플레이어가 공격되는 현상 없애기
        List<GameObject> objList = Managers.Game.GetPlayer().GetComponent<PlayerController>().charEquipment[armorId];
        foreach(GameObject obj in objList)
            obj.SetActive(isActive);
    }

    public override void ClearSlot()
    {
        base.ClearSlot();

        armorItem = null;
    }
}
