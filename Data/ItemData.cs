using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public int id;
    public string itemName;
    public Define.ItemType itemType = Define.ItemType.Unknown;
    public Define.itemGrade itemGrade = Define.itemGrade.Common;
    public int itemPrice;
    public int itemMaxCount = 99;

    public GameObject itemObject;

    [TextArea]
    public string itemDesc;
    
    public Sprite itemIcon;
}
