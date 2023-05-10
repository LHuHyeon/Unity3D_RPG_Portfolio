using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
