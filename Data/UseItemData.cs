using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 소비 아이템 데이터 ]
1. 모든 소비 아이템은 해당 클래스를 받음.
2. 체력 or 마나를 회복한다.
*/

[Serializable]
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
