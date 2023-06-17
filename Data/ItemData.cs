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
        ItemData item = new ItemData();
        
        item.id = this.id;
        item.itemName = this.itemName;
        item.itemType = this.itemType;
        item.itemGrade = this.itemGrade;
        item.itemPrice = this.itemPrice;
        item.itemMaxCount = this.itemMaxCount;
        item.itemObject = this.itemObject;
        item.itemDesc = this.itemDesc;
        item.itemIcon = this.itemIcon;

        if (this is EquipmentData)
        {
            EquipmentData equip = item as EquipmentData;
            equip = (this as EquipmentData).EquipmentClone();
            
            if (this is ArmorItemData)
            {
                ArmorItemData armor = item as ArmorItemData;
                armor = (this as ArmorItemData).ArmorClone();

                // TODO : 여기 부터 구현 시작
                Debug.Log("armor minlevel : "+ (item as ArmorItemData).minLevel);
            }
            else if (this is WeaponItemData)
            {
                WeaponItemData weapon = item as WeaponItemData;
                weapon = (this as WeaponItemData).WeaponClone();
            }
        }
        else if (this is UseItemData)
        {
            UseItemData useItem = item as UseItemData;
            useItem = (this as UseItemData).UseClone();
        }

        return item;
    }
}
