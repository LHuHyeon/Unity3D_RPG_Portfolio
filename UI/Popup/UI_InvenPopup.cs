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

    [SerializeField]
    int invenCount = 42;    // 인벤 슬롯 개수

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        Managers.Input.KeyAction -= OnInventoryUI;
        Managers.Input.KeyAction += OnInventoryUI;

        SetInfo();

        AcquireItem(Managers.Data.Item[2001]);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    void Update()
    {
        if (Managers.Game.isInventory == true)
            RefreshUI();
    }

    // 인벤토리 활성화
    void OnInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.Game.isInventory = !Managers.Game.isInventory;

            if (Managers.Game.isInventory)
            {
                RefreshUI();
                Managers.UI.OnPopupUI(this);
            }
            else
                Managers.UI.ClosePopupUI(this);
        }
    }

    // 인벤 슬롯 아이템 넣기
    public void AcquireItem(ItemData item, int count = 1)
    {
        foreach(UI_InvenItem slot in Managers.Game.InvenSlots)
        {
            // 아이템이 없으면 넣기
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
        Managers.Game.InvenSlots = new List<UI_InvenItem>();

        foreach(Transform child in grid.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<invenCount; i++)
            Managers.Game.InvenSlots.Add(Managers.UI.MakeSubItem<UI_InvenItem>(parent: grid.transform));
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
                
            Managers.Game.isInventory = false;
            Managers.UI.ClosePopupUI(this);
        }, Define.UIEvent.Click);
    }

    void RefreshUI()
    {   
        GetText((int)Texts.GoldText).text = Managers.Game.Gold.ToString();
    }
}
