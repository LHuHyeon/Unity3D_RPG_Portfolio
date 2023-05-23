using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EqStatPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Title,
        EqSlot,
        ExitButton,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));

        Managers.Input.KeyAction -= OnEquipmentUI;
        Managers.Input.KeyAction += OnEquipmentUI;

        SetInfo();

        gameObject.SetActive(false);

        return true;
    }

    void OnEquipmentUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.Game.isEquipment = !Managers.Game.isEquipment;

            Managers.Game._equipment.gameObject.SetActive(Managers.Game.isEquipment);
        }
    }

    public void SetInfo()
    {
        // GetSlot();
        SetEventHandler();
    }

    void GetSlot()
    {
        Managers.Game.CurrentArmor = new Dictionary<Define.ArmorType, ArmorItemData>();
        
        foreach(Transform child in GetObject((int)Gameobjects.EqSlot).transform)
        {
            UI_ArmorItem eqSlot = child.GetComponent<UI_ArmorItem>();
            // Managers.Game.CurrentArmor.Add(eqSlot.armorType, eqSlot.armorItem);
        }
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform eqStatPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            eqStatPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(eqStatPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(eqStatPos.anchoredPosition.y + eventData.delta.y, -253, 217)
            );
        }, Define.UIEvent.Drag);

        // Order 설정
        GetObject((int)Gameobjects.Background).BindEvent((PointerEventData eventData)=>
        {
            Managers.UI.SetOrder(GetComponent<Canvas>());
        }, Define.UIEvent.Click);

        // Exit 버튼
        GetObject((int)Gameobjects.ExitButton).BindEvent((PointerEventData eventData)=>
        {
            Managers.Game.isInventory = false;
            Managers.Game._inventory.gameObject.SetActive(Managers.Game.isInventory);
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {   

    }
}
