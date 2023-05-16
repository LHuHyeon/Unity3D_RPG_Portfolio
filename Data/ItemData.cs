using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData
{
    public int id;
    public string itemName;
    public Define.itemType itemType = Define.itemType.Unknown;
    public Define.itemGrade itemGrade = Define.itemGrade.Common;
    public int itemPrice;
    public int itemMaxCount = 99;
    
    // TODO : 기타 아이템은 부스러기 모양으로 통일
    public GameObject itemObject;

    [TextArea]
    public string itemDesc;
    
    public Sprite itemIcon;
}
