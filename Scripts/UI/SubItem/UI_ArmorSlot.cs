using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * File :   UI_ArmorSlot.cs
 * Desc :   UI_EqStatPopup.cs의 하위객체에서 사용되며 방어구 아이템을 장착/해제할 수 있다.
 *
 & Functions
 &  [Public]
 &  : SetInfo()         - 기능 설정
 &  : ChangeArmor()     - 방어구 장착 및 교체
 &  : AddItem()         - 아이템 추가
 &  : ClearSlot()       - 초기화
 &
 &  [Protected]
 &  : OnClickSlot()     - 슬롯 우클릭 시 "장비 해제"
 &  : OnEndDragSlot()   - 마우스 클릭을 해제한 위치가 UI라면 "인벤으로 보내기"
 &  : OnDropSlot()      - 현재 슬롯에 마우스 클릭을 때면 "장비 장착"
 &  : ChangeSlot()      - 슬롯 교체
 &
 &  [Private]
 &  : AddArmor()        - 장비 장착 진행
 &  : EquipmentActive() - 장비 파츠 확인
 *
 */

public class UI_ArmorSlot : UI_ItemDragSlot
{
    public Define.ArmorType     armorType = Define.ArmorType.Unknown;
    public ArmorItemData        armorItem;

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

    // 방어구 교체
    public void ChangeArmor(UI_ItemSlot itemSlot)
    {
        ChangeSlot(itemSlot); 
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

    protected override void OnClickSlot(PointerEventData eventData)
    {
        if (item.IsNull() == true || UI_DragSlot.instance.dragSlotItem.IsNull() == false)
            return;

        // 우클릭하여 장비 벗기
        if (Input.GetMouseButtonUp(1))
        {
            // 인벤으로 보내고 초기화
            if (Managers.Game._playScene._inventory.AcquireItem(armorItem) == true)
                ClearSlot();
        }
    }

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

    protected override void ChangeSlot(UI_ItemSlot itemSlot)
    {
        // 장비 확인
        if ((itemSlot.item is ArmorItemData) == false)
            return;

        // 같은 부위 확인
        ArmorItemData armor = itemSlot.item as ArmorItemData;
        if (armorType != armor.armorType)
            return;

        // 레벨 확인
        if (Managers.Game.Level < armor.minLevel)
        {
            Managers.UI.MakeSubItem<UI_Guide>().SetInfo("레벨이 부족합니다.", new Color(1f, 0.5f, 0f));
            return;
        }

        ItemData _tempItem = item;

        // 장비 장착
        AddItem(itemSlot.item);

        // 기존 장비 인벤 이동
        UI_InvenSlot inven = itemSlot as UI_InvenSlot;
        if (_tempItem.IsNull() == false)
            inven.AddItem(_tempItem);
        else
            inven.ClearSlot();
    }

    // 장비 장착
    private void AddArmor(ArmorItemData armorItem)
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
    private void EquipmentActive(ArmorItemData armor, bool isActive)
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
