using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopSaleItem : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        SaleItemIcon,
    }

    enum Texts
    {
        SaleItemCountText,
    }

    UI_InvenItem _invenItem;
    int _saleItemCount = 0;

    Image _icon;
    string _itemCountText;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClickCloseButton);

        GetImage((int)Images.SaleItemIcon).sprite = _icon.sprite;
        GetText((int)Texts.SaleItemCountText).text = _itemCountText;

        return true;
    }

    public void SetInfo(UI_InvenItem invenItem)
    {
        _invenItem = invenItem;
        _saleItemCount = _invenItem.subItemCount;

        _invenItem.IsLock = true;

        _icon = _invenItem.icon;
        if (invenItem.item is UseItemData)
            _itemCountText = _saleItemCount.ToString();
        else
            _itemCountText = "";
    }

    // 판매 진행
    public void GetSale()
    {
        Managers.Game.Gold += _invenItem.item.itemPrice * _saleItemCount;
        _invenItem.SetCount(-_saleItemCount);
        OnClickCloseButton();
    }

    void OnClickCloseButton()
    {
        _invenItem.subItemCount = 0;
        _invenItem.IsLock = false;
        Managers.Game._playScene._shop.saleList.Remove(this);

        Managers.Resource.Destroy(gameObject);
    }
}
