using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopSaleItem : UI_Base
{
    enum Buttons
    {
        CloseButton,
    }

    enum Images
    {
        Saleicon,
    }

    enum Texts
    {
        SaleItemCountText,
    }

    UI_InvenItem _invenItem;
    int _saleItemCount = 0;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(OnClickCloseButton);

        return true;
    }

    public void SetInfo(UI_InvenItem invenItem, int saleItemCount)
    {
        _invenItem = invenItem;
        _saleItemCount = saleItemCount;

        GetImage((int)Images.Saleicon).sprite = _invenItem.icon.sprite;
        GetText((int)Texts.SaleItemCountText).text = _saleItemCount.ToString();
    }

    void OnClickCloseButton()
    {
        _invenItem.SetCount(_saleItemCount);
        Managers.Resource.Destroy(gameObject);
    }
}
