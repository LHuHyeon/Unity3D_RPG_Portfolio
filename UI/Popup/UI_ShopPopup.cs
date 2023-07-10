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

    enum Gameobjects
    {
        Title,
        Background,
        ExitButton,
        BuyButton,
        SaleButton,
        BuyList,
        SaleList,
        GoSaleButton,
    }

    enum Texts
    {
        TitleText,
    }

    public Define.ShopType shopType = Define.ShopType.Unknown;

    List<UI_ShopBuyItem> buyList;       // 구매 슬롯
    public List<UI_ShopSaleItem> saleList;     // 판매 슬롯

    int currentItemId = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        SetInfo();

        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

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

        SetEventHandler();
    }

    public void RefreshUI(ShopNpcController npc, int shopBuyId)
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

        SettingBuySlot(shopBuyId);
    }

    void SettingBuySlot(int shopBuyId)
    {
        // 똑같은 상점에 들린다면
        if (currentItemId == shopBuyId)
            return;

        currentItemId = shopBuyId;

        // 구매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 구매 Id 가져오기
        List<int> itemIdList = new List<int>();
        if (Managers.Data.Shop.TryGetValue(shopBuyId, out itemIdList) == false)
        {
            Debug.Log("Shop ItemIdList Failed!");
            return;
        }

        // 구매 슬롯 채우기
        for(int i=0; i<itemIdList.Count; i++)
        {
            UI_ShopBuyItem buyShop = Managers.UI.MakeSubItem<UI_ShopBuyItem>(parent: GetObject((int)Gameobjects.BuyList).transform);
            buyShop.SetInfo(Managers.Data.Item[itemIdList[i]]);
            buyList.Add(buyShop);
        }
    }

    void SetEventHandler()
    {
        // Title 잡고 인벤토리 이동
        RectTransform shopPos = GetObject((int)Gameobjects.Background).GetComponent<RectTransform>();
        GetObject((int)Gameobjects.Title).BindEvent((PointerEventData eventData)=>
        {
            shopPos.anchoredPosition = new Vector2
            (
                Mathf.Clamp(shopPos.anchoredPosition.x + eventData.delta.x, -655, 655),
                Mathf.Clamp(shopPos.anchoredPosition.y + eventData.delta.y, -253, 217)
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
            ExitShop();
        }, Define.UIEvent.Click);

        // 판매할 아이템 받기
        GetObject((int)Gameobjects.SaleList).BindEvent((PointerEventData eventData)=>
        {
            UI_SlotItem dragSlot = UI_DragSlot.instance.dragSlotItem;

            // 인벤토리 슬롯 확인
            if (dragSlot is UI_InvenItem == false)
                return;

            UI_InvenItem invenItem = dragSlot as UI_InvenItem;

            // 장비거나 개수가 한개라면 바로 넣기
            if (invenItem.item is EquipmentData || invenItem.itemCount == 1)
            {
                SaleItemRegister(dragSlot as UI_InvenItem);
            }
            else
            {
                // 판매 개수 선택
                UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
                numberCheckPopup.RefreshUI(invenItem, (int subItemCount)=>
                {
                    // 판매 슬롯 생성
                    UI_ShopSaleItem saleItem = Managers.UI.MakeSubItem<UI_ShopSaleItem>(GetObject((int)Gameobjects.SaleList).transform);
                    saleItem.SetInfo((dragSlot as UI_InvenItem), subItemCount);
                    saleList.Add(saleItem);
                });
            }
        }, Define.UIEvent.Drop);

        GetObject((int)Gameobjects.BuyButton).BindEvent(OnClickBuyListButton);
        GetObject((int)Gameobjects.SaleButton).BindEvent(OnClickSaleListButton);
        GetObject((int)Gameobjects.GoSaleButton).BindEvent(OnClickGoSaleButton);
    }

    void OnClickBuyListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);
    }

    void OnClickSaleListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(false);
        GetObject((int)Gameobjects.SaleList).SetActive(true);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(true);
    }

    // 판매 진행 버튼
    void OnClickGoSaleButton(PointerEventData eventData)
    {
        if (saleList.Count == 0)
            return;

        for(int i=0; i<saleList.Count; i++)
            saleList[i].GetSale();

        saleList.Clear();
    }

    void SaleItemRegister(UI_InvenItem invenItem)
    {
        UI_ShopSaleItem saleItem = Managers.UI.MakeSubItem<UI_ShopSaleItem>(GetObject((int)Gameobjects.SaleList).transform);
        saleItem.SetInfo(invenItem);
        saleList.Add(saleItem);
    }

    public void ExitShop()
    {
        Managers.Game._playScene._slotTip.OnSlotTip(false);
        
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

        Managers.Game.IsInteract = false;
        Managers.UI.ClosePopupUI(this);

        Managers.Game.isPopups[Define.Popup.Inventory] = false;
        Managers.UI.ClosePopupUI(Managers.Game._playScene._inventory);
    }
}
