using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopPopup : UI_Popup
{
    /*
    1. UI_ShopBuyItem 초기화 후 상점 판매 아이템에 맞게 생성
    2. UI_ShopSaleItem 초기화 후 판매 물품 들어오면 생성
    3. 
    */

    public enum Gameobjects
    {
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

    List<UI_ShopBuyItem> buyList;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(Gameobjects));
        BindText(typeof(Texts));

        Managers.UI.ClosePopupUI(this);

        return true;
    }

    public void SetInfo()
    {
        buyList = new List<UI_ShopBuyItem>();

        foreach(Transform child in GetObject((int)Gameobjects.BuyList).transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<buyCount; i++)
        {
            UI_ShopBuyItem buyShop = Managers.UI.MakeSubItem<UI_ShopBuyItem>(parent: GetObject((int)Gameobjects.BuyList).transform);
            buyList.Add(buyShop);
        }
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
}
