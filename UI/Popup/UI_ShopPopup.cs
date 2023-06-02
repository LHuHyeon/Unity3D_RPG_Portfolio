using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ShopPopup : UI_Popup
{
    /*
    1. UI_ShopBuyItem 초기화 후 상점 판매 아이템에 맞게 생성
    2. UI_ShopSaleItem 초기화 후 판매 물품 들어오면 생성
    3. 
    */

    public enum Gameobjects
    {
        Title,
        Background,
        ExitButton,
        BuyButton,
        SaleButton,
        BuyList,
        SaleList,
    }

    public enum Texts
    {
        TitleText,
    }

    public Define.ShopType shopType = Define.ShopType.Unknown;

    [SerializeField]
    int buyCount = 10;

    List<UI_ShopBuyItem> buyList;       // 구매 슬롯
    List<UI_ShopSaleItem> saleList;     // 판매 슬롯

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        SetInfo();

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void SetInfo()
    {
        buyList = new List<UI_ShopBuyItem>();
        saleList = new List<UI_ShopSaleItem>();

        // 구매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.SaleList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 판매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 구매 슬롯 공간 채우기
        for(int i=0; i<buyCount; i++)
        {
            UI_ShopBuyItem buyShop = Managers.UI.MakeSubItem<UI_ShopBuyItem>(parent: GetObject((int)Gameobjects.BuyList).transform);
            buyList.Add(buyShop);
        }

        SetEventHandler();
    }

    public void RefreshUI(ShopNpcController npc)
    {
        shopType = npc.shopType;

        if (shopType == Define.ShopType.Used)
            GetText((int)Texts.TitleText).text = "Used Shop";
        else if (shopType == Define.ShopType.Armor)
            GetText((int)Texts.TitleText).text = "Armor Shop";
        else if (shopType == Define.ShopType.Weapon)
            GetText((int)Texts.TitleText).text = "Weapon Shop";
        else if (shopType == Define.ShopType.Accessory)
            GetText((int)Texts.TitleText).text = "Accessory Shop";
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform invenPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
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
            Managers.Game.IsInteract = false;
            Managers.UI.ClosePopupUI(this);

            Managers.Game.isInventory = false;
            Managers.UI.ClosePopupUI(Managers.Game._playScene._inventory);
        }, Define.UIEvent.Click);

        // 판매할 아이템 받기
        GetObject((int)Gameobjects.SaleList).BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            // 인벤토리 슬롯 확인
            if (dragSlot is UI_InvenItem == false)
                return;

            UI_InvenItem invenItem = dragSlot as UI_InvenItem;

            // 판매 개수 선택
            int itemCount = invenItem.itemCount;
            UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            numberCheckPopup.RefreshUI(invenItem, ()=>
            {
                // 이 함수가 호출되면 차감된 슬롯 아이템 개수를 itemCount에 적용
                itemCount -= invenItem.itemCount;

                // 판매 슬롯 생성
                UI_ShopSaleItem saleItem = Managers.UI.MakeSubItem<UI_ShopSaleItem>(GetObject((int)Gameobjects.SaleList).transform);
                saleItem.SetInfo(dragSlot as UI_InvenItem, itemCount);
                saleList.Add(saleItem);
            });

        }, Define.UIEvent.Drop);
    }
}
