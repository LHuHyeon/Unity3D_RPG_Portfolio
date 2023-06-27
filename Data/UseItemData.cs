using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemData : ItemData
{
    public Define.UseType useType = Define.UseType.Unknown;
    public int useValue = 0;
    public int itemCount = 0;

    public bool UseItem(ItemData item)
    {
        if ((item is UseItemData) == false)
            return false;

        UseItemData useItem = item as UseItemData;

        if (useItem.useType == Define.UseType.Hp)
            Managers.Game.Hp += useItem.useValue;
        else if (useItem.useType == Define.UseType.Mp)
            Managers.Game.Mp += useItem.useValue;

        return true;
    }

    public UseItemData UseClone()
    {
        UseItemData useItem = new UseItemData();
        useItem.useType = this.useType;
        useItem.useValue = this.useValue;
        useItem.itemCount = this.itemCount;

        return useItem;
    }
}
