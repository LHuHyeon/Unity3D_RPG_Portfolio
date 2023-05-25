using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ShopSaleItem : UI_Base
{
    public enum Images
    {
        Saleicon,
    }

    public enum Texts
    {
        SaleItemCountText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
