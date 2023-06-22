using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ShopBuyItem : UI_Base
{
    public enum Images
    {
        BuyItemImage,
    }

    public enum Texts
    {
        BuyItemName,
        BuyItemPrice,
    }

    ItemData _item;

    Sprite buySprite;
    string itemNameText;
    string itemPriceText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        gameObject.BindEvent(OnClickBuyButton);

        GetImage((int)Images.BuyItemImage).sprite = buySprite;
        GetText((int)Texts.BuyItemName).text = itemNameText;
        GetText((int)Texts.BuyItemPrice).text = itemPriceText;

        return true;
    }

    public void SetInfo(ItemData item)
    {
        _item = item;

        buySprite = item.itemIcon;
        itemNameText = item.itemName;
        itemPriceText = item.itemPrice.ToString();
    }

    void OnClickBuyButton(PointerEventData eventData)
    {
        if (Managers.Game.Gold < _item.itemPrice)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }
        
        // 사용 아이템이면 개수 선택
        if (_item.itemType == Define.ItemType.Use)
        {
            UI_NumberCheckPopup numberCheckPopup = Managers.UI.ShowPopupUI<UI_NumberCheckPopup>();
            numberCheckPopup.RefreshUI(_item, (int itemCount)=>
            {
                Managers.Game.Gold -= _item.itemPrice * itemCount;
                Managers.Game._playScene._inventory.AcquireItem(_item.ItemClone(), itemCount);
            });
        }
        else
        {
            Managers.UI.ShowPopupUI<UI_ConfirmPopup>().SetInfo(()=>
            {
                Managers.Game.Gold -= _item.itemPrice;
                Managers.Game._playScene._inventory.AcquireItem(_item.ItemClone());
            }, Define.ShopSaleMessage);
        }
    }
}
