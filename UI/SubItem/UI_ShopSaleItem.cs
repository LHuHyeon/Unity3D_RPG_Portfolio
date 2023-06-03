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
        _itemCountText = _saleItemCount.ToString();
    }

    // 판매 진행
    public void GetSale()
    {
        Managers.Game.Gold += _invenItem.item.itemPrice;
        _invenItem.SetCount(-_saleItemCount);
        Managers.Resource.Destroy(gameObject);
    }

    void OnClickCloseButton()
    {
        _invenItem.subItemCount = 0;
        _invenItem.IsLock = false;
        Managers.Resource.Destroy(gameObject);
    }
}
