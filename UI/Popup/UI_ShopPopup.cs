using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
[ 상점 Popup 스크립트 ]
1. 장비, 소비 등의 아이템을 구매/판매할 수 있는 Popup이다.
2. 해당 클래스는 Popup만 활성화/비활성화 해주면 그 안의 슬롯들이 기능을 담당하고 있다.
3. 슬롯 : UI_ShopSaleItem.cs(판매 슬롯), UI_ShopBuyItem.cs(구매 슬롯)
*/

public class UI_ShopPopup : UI_Popup
{
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

        // 판매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.SaleList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 구매 슬롯 초기화
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
            UI_Slot dragSlot = UI_DragSlot.instance.dragSlotItem;

            // 인벤토리 슬롯 확인
            if (dragSlot is UI_InvenItem == true)
                SetSaleItemRegister(dragSlot as UI_InvenItem);

        }, Define.UIEvent.Drop);

        // 우클릭으로 판매할 아이템 받기
        Managers.Game._getSlotInteract -= GetSlotInteract;
        Managers.Game._getSlotInteract += GetSlotInteract;

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

        int beforeGold = Managers.Game.Gold;
        for(int i=0; i<saleList.Count; i++)
            saleList[i].GetSale();

        int afterGold = Managers.Game.Gold - beforeGold;
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo($"Gold {afterGold}+", Color.yellow);

        saleList.Clear();
    }

    // 우클릭 아이템 받기
    void GetSlotInteract(UI_InvenItem invenSlot)
    {
        // 판매 리스트가 현재 활성화 중이라면
        if (GetObject((int)Gameobjects.SaleList).activeSelf == true)
            SetSaleItemRegister(invenSlot);
    }

    // 판매 아이템 등록
    void SetSaleItemRegister(UI_InvenItem invenSlot)
    {
        // 장비거나 개수가 한개라면 바로 넣기
        if (invenSlot.item is EquipmentData || invenSlot.itemCount == 1)
        {
            SaleItemRegister(invenSlot);
        }
        else
        {
            // 판매 개수 선택
            UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            if (numberCheckPopup.IsNull() == true) return;

            numberCheckPopup.RefreshUI(invenSlot, (int subItemCount)=>
            {
                SaleItemRegister(invenSlot, subItemCount);
            });
        }
    }

    void SaleItemRegister(UI_InvenItem invenItem, int count = 1)
    {
        UI_ShopSaleItem saleItem = Managers.UI.MakeSubItem<UI_ShopSaleItem>(GetObject((int)Gameobjects.SaleList).transform);
        saleItem.SetInfo(invenItem, count);
        saleList.Add(saleItem);
    }

    public void ExitShop()
    {
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

        for(int i=0; i<saleList.Count; i++)
            saleList[i].Clear();

        saleList.Clear();

        Managers.Game.IsInteract = false;

        Managers.UI.CloseAllPopupUI();
    }
}
