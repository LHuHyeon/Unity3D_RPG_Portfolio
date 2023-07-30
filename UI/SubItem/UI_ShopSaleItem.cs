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

    public void SetInfo(UI_InvenItem invenItem, int subItemCount = 1)
    {
        _invenItem = invenItem;
        _saleItemCount = subItemCount;

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
        if ((_invenItem.item is EquipmentData) == true)
        {
            EquipmentData equipment = _invenItem.item as EquipmentData;
            Managers.Game.Gold += _invenItem.item.itemPrice + (int)((equipment.itemPrice / 4) * (equipment.upgradeCount));
        }
        else
            Managers.Game.Gold += _invenItem.item.itemPrice * _saleItemCount;

        _invenItem.SetCount(-_saleItemCount);

        Clear();
    }

    void OnClickCloseButton()
    {
        Managers.Game._playScene._shop.saleList.Remove(this);

        Clear();
    }

    public void Clear()
    {
        _invenItem.IsLock = false;

        Managers.Resource.Destroy(this.gameObject);
    }
}
