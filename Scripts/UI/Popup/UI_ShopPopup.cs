using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * File :   UI_ShopPopup.cs
 * Desc :   장비, 소비 등의 아이템을 구매/판매할 수 있는 Popup UI
 *
 & Functions
 &  [Public]
 &  : Init()        - 초기 설정
 &  : RefreshUI()   - 새로고침 UI (SettingBuySlot() 호출)
 &  : ExitShop()    - 상점 나가기 (초기화)
 &
 &  [Private]
 &  : SettingBuySlot()          - 구매 슬롯 설정
 &  : OnClickBuyListButton()    - 구매 리스트 호출 버튼
 &  : OnClickSaleListButton()   - 판매 호출 버튼
 &  : OnClickGoSaleButton()     - 판매 진행 버튼
 &  : SetSaleItemRegister()     - 판매 개수 확인 후 등록 (SaleItemRegister() 호출)
 &  : SaleItemRegister()        - 판매 등록
 &  : SetInfo()                 - 기능 설정
 &  : SetEventHandler()         - EventHandler 설정
 &  : GetSlotInteract()         - 인벤토리와 상호작용 (우클릭 아이템 받기)
 *
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

    public Define.ShopType          shopType = Define.ShopType.Unknown;

    public List<UI_ShopSaleSlot>    saleList;     // 판매 슬롯
    private List<UI_ShopBuySlot>    buyList;       // 구매 슬롯

    private int                     currentShopId = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        // 자식 객체 불러오기
        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        SetInfo();

        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void RefreshUI(ShopNpcController npc, int shopBuyId)
    {
        // 상점 이름 설정
        GetText((int)Texts.TitleText).text = $"{npc.shopType.ToString()} Shop";

        // 구매 슬롯 설정
        SettingBuySlot(shopBuyId);
    }

    // 구매 슬롯 설정
    private void SettingBuySlot(int shopBuyId)
    {
        // 똑같은 상점에 들린다면
        if (currentShopId == shopBuyId)
            return;

        currentShopId = shopBuyId;

        // 구매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 구매 Id List 가져오기
        List<int> itemIdList = new List<int>();
        if (Managers.Data.Shop.TryGetValue(shopBuyId, out itemIdList) == false)
        {
            Debug.Log("Shop ItemIdList Failed!");
            return;
        }

        // 구매 슬롯 채우기
        for(int i=0; i<itemIdList.Count; i++)
        {
            UI_ShopBuySlot buyShop = Managers.UI.MakeSubItem<UI_ShopBuySlot>(parent: GetObject((int)Gameobjects.BuyList).transform);
            buyShop.SetInfo(Managers.Data.Item[itemIdList[i]]);
            buyList.Add(buyShop);
        }
    }

    // 구매 리스트 호출 버튼
    private void OnClickBuyListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(true);
        GetObject((int)Gameobjects.SaleList).SetActive(false);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(false);
    }

    // 판매 호출 버튼
    private void OnClickSaleListButton(PointerEventData eventData)
    {
        GetObject((int)Gameobjects.BuyList).SetActive(false);
        GetObject((int)Gameobjects.SaleList).SetActive(true);
        GetObject((int)Gameobjects.GoSaleButton).SetActive(true);
    }

    // 판매 진행 버튼
    private void OnClickGoSaleButton(PointerEventData eventData)
    {
        // 판매 등록 확인
        if (saleList.Count == 0)
            return;

        // 팔기 전 골드 저장
        int beforeGold = Managers.Game.Gold;

        // 아이템 팔기
        for(int i=0; i<saleList.Count; i++)
            saleList[i].GetSale();

        // 판매 후 골드 저장
        int afterGold = Managers.Game.Gold - beforeGold;

        // 획득한 골드 안내문 생성
        Managers.UI.MakeSubItem<UI_Guide>().SetInfo($"Gold {afterGold}+", Color.yellow);

        // 초기화
        saleList.Clear();
    }

    // 판매 아이템 등록
    private void SetSaleItemRegister(UI_InvenSlot invenSlot)
    {
        // 장비거나 개수가 한개라면 판매 등록
        if (invenSlot.item is EquipmentData || invenSlot.itemCount == 1)
        {
            SaleItemRegister(invenSlot);
            return;
        }

        // 판매 개수 선택
        UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
        if (numberCheckPopup.IsNull() == true) return;

        // 개수 선택 설정
        numberCheckPopup.SetInfo(invenSlot, (int subItemCount)=>
        {
            // 개수 선택한 만큼 판매 등록
            SaleItemRegister(invenSlot, subItemCount);
        });
    }

    // 판매 등록
    private void SaleItemRegister(UI_InvenSlot invenItem, int count = 1)
    {
        // 판매 슬로 생성 후 아이템 등록
        UI_ShopSaleSlot saleItem = Managers.UI.MakeSubItem<UI_ShopSaleSlot>(GetObject((int)Gameobjects.SaleList).transform);
        saleItem.SetInfo(invenItem, count);
        saleList.Add(saleItem);
    }

    private void SetInfo()
    {
        buyList = new List<UI_ShopBuySlot>();
        saleList = new List<UI_ShopSaleSlot>();

        // 판매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.SaleList).transform)
            Managers.Resource.Destroy(child.gameObject);

        // 구매 슬롯 초기화
        foreach(Transform child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(child.gameObject);

        SetEventHandler();
    }

    private void SetEventHandler()
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
            if (dragSlot is UI_InvenSlot == true)
                SetSaleItemRegister(dragSlot as UI_InvenSlot);

        }, Define.UIEvent.Drop);

        // 우클릭으로 판매할 아이템 받기
        Managers.Game._getSlotInteract -= GetSlotInteract;
        Managers.Game._getSlotInteract += GetSlotInteract;

        GetObject((int)Gameobjects.BuyButton).BindEvent(OnClickBuyListButton);
        GetObject((int)Gameobjects.SaleButton).BindEvent(OnClickSaleListButton);
        GetObject((int)Gameobjects.GoSaleButton).BindEvent(OnClickGoSaleButton);
    }

    // 우클릭 아이템 받기
    private void GetSlotInteract(UI_InvenSlot invenSlot)
    {
        // 판매 리스트가 현재 활성화 중이라면
        if (GetObject((int)Gameobjects.SaleList).activeSelf == true)
            SetSaleItemRegister(invenSlot);
    }

    // 상점 나가기 (초기화)
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
