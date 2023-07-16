using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InvenPopup : UI_Popup
{
    enum Gameobjects
    {
        Background,
        Content,
        Title,
        ExitButton,
    }

    enum Texts
    {
        GoldText,
    }

    List<UI_InvenItem> invenSlots;

    [SerializeField]
    int invenCount = 42;    // 인벤 슬롯 개수

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        invenSlots = new List<UI_InvenItem>();
        popupType = Define.Popup.Inventory;

        Managers.Input.KeyAction -= OnInventoryUI;
        Managers.Input.KeyAction += OnInventoryUI;

        SetInfo();

        Invoke("DelayInit", 0.0001f);

        return true;
    }
    
    void DelayInit() { Managers.UI.ClosePopupUI(this); }

    void Update()
    {
        if (Managers.Game.isPopups[Define.Popup.Inventory] == true)
            RefreshUI();
    }

    // 인벤토리 활성화
    void OnInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isPopups[Define.Popup.Inventory] = !Managers.Game.isPopups[Define.Popup.Inventory];

            if (Managers.Game.isPopups[Define.Popup.Inventory])
            {
                RefreshUI();
                Managers.UI.OnPopupUI(this);
            }
            else
                Exit();
        }
    }

    // 인벤 슬롯 아이템 넣기
    public void AcquireItem(ItemData item, int count = 1)
    {
        foreach(UI_InvenItem slot in invenSlots)
        {
            // 슬롯에 아이템이 없으면 넣기
            if (slot.item == null)
            {
                slot.AddItem(item, count);
                break;
            }

            // 사용 아이템이라면 (기타 아이템은 필요 시 추가)
            if (item is UseItemData)
            {
                // 같은 아이템이 존재하면 개수 추가
                if (item.id == slot.item.id)
                {
                    slot.SetCount(count);
                    break;
                }
            }
        }
    }

    public void ResetPos()
    {
        RectTransform invenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        invenPos.anchoredPosition = new Vector2(330, 0);
    }

    public void SetInfo()
    {
        ResetSlot();
        SetEventHandler();
    }

    void ResetSlot()
    {
        // 슬롯 초기화
        GameObject grid = GetObject((int)Gameobjects.Content);

        foreach(Transform child in grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<invenCount; i++)
        {
            UI_InvenItem invenItem = Managers.UI.MakeSubItem<UI_InvenItem>(parent: grid.transform);
            invenItem.invenNumber = i;

            if (Managers.Game.InvenItem.TryGetValue(i, out ItemData item) == true)
            {
                // 소비 아이템이라면 개수 반영
                if (item is UseItemData)
                {
                    UseItemData useItem = item as UseItemData;
                    invenItem.AddItem(item, useItem.itemCount);
                }
                else
                    invenItem.AddItem(item);
            }

            invenSlots.Add(invenItem);
        }
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform invenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            // 상호작용 중이면 이동X
            if (Managers.Game.IsInteract == true)
                return;

            invenPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(invenPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(invenPos.anchoredPosition.y + eventData.delta.y, -253, 217)
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
            if (Managers.Game.IsInteract == true)
                return;
                
            Exit();
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {   
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }

    void Exit()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        Managers.UI.ClosePopupUI(this);
    }
}
