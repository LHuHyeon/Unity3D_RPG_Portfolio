using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
[ 방어구 Slot 스크립트 ]
1. EqStatPopup안에 있는 방어구 Slot으로 장비를 장착/해제할 수 있다.
2. 드래그 드랍 or 우클릭으로 장비 장착이 가능하다.
*/

public class UI_ArmorItem : UI_ItemDragSlot
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;
    public ArmorItemData armorItem;

    public override void SetInfo()
    {
        Managers.Game._playScene._equipment.armorSlots.Add(this);

        // 해당 부위 장비가 이미 장착되어 있다면 장착 (Save Load 했을때)
        if (Managers.Game.CurrentArmor.TryGetValue(armorType, out armorItem) == true)
        {
            base.AddItem(armorItem);
            AddArmor(armorItem);
        }

        base.SetInfo();
    }

    // 우클릭하여 장비 벗기
    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            return;

        if (Input.GetMouseButtonUp(1))
        {
            // 인벤으로 보내고 초기화
            if (Managers.Game._playScene._inventory.AcquireItem(armorItem) == true)
                ClearSlot();
        }
    }

    // 드래그 끝날 때
    protected override void OnEndDragSlot(PointerEventData eventData)
    {
        // 아이템을 버린 위치가 UI가 아니라면
        if (item.IsNull() == false && !EventSystem.current.IsPointerOverGameObject())
        {
            // 인벤으로 보내고 초기화
            if (Managers.Game._playScene._inventory.AcquireItem(armorItem) == true)
                ClearSlot();
        }
        
        base.OnEndDragSlot(eventData);
    }

    // Slot을 Drop 받을 때
    protected override void OnDropSlot(PointerEventData eventData)
    {
        UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

        if (dragSlot.IsNull() == false)
        {
            // 자기 자신이라면 취소
            if (dragSlot == this)
                return;

            // 장비 장착 (or 교체)
            ChangeSlot(dragSlot as UI_ItemSlot);
        }
    }

    public void ChangeArmor(UI_ItemSlot itemSlot) { ChangeSlot(itemSlot); }

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
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
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            return;
        }

        ItemData _tempItem = item;

        // 장비 장착
        AddItem(itemSlot.item);

        // 기존 장비 인벤 이동
        UI_InvenItem inven = itemSlot as UI_InvenItem;
        if (_tempItem.IsNull() == false)
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

        // 방어구 장착
        AddArmor(armorItem);
    }

    // 장비 장착
    void AddArmor(ArmorItemData armorItem)
    {
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

    // 캐릭터 장비 파츠 활성화 여부
    void EquipmentActive(ArmorItemData armor, bool isActive)
    {
        // 아이템이 현재 입고 있는 장비를 알고 있다면
        if (armor.charEquipment.IsNull() == false)
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
        Managers.Game.CurrentArmor.Remove(armorType);
    }
}
