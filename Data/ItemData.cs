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

    // Deep Copy (깊은 복사)
    public ItemData ItemClone()
    {
        if (this is EquipmentData)
        {
            AddItemValue<EquipmentData>((this as EquipmentData).EquipmentClone());
            
            if (this is ArmorItemData)
            {
                return AddItemValue<ArmorItemData>((this as ArmorItemData).ArmorClone());
            }
            else if (this is WeaponItemData)
            {
                return AddItemValue<WeaponItemData>((this as WeaponItemData).WeaponClone());
            }
        }
        else if (this is UseItemData)
        {
            return AddItemValue<UseItemData>((this as UseItemData).UseClone());
        }

        return AddItemValue<ItemData>();
    }

    T AddItemValue<T>(T item = null) where T : ItemData, new()
    {
        if (item == null)
            item = new T();

        item.id = this.id;
        item.itemName = this.itemName;
        item.itemType = this.itemType;
        item.itemGrade = this.itemGrade;
        item.itemPrice = this.itemPrice;
        item.itemMaxCount = this.itemMaxCount;
        item.itemObject = this.itemObject;
        item.itemDesc = this.itemDesc;
        item.itemIcon = this.itemIcon;

        return item;
    }
}
