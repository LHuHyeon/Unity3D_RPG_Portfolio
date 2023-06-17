using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemData : ItemData
{
    public Define.UseType useType = Define.UseType.Unknown;
    public int useValue = 0;

    public UseItemData UseClone()
    {
        UseItemData useItem = new UseItemData();
        useItem.useType = this.useType;
        useItem.useValue = this.useValue;

        return useItem;
    }
}
