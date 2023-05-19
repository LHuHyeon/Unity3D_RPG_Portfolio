using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemData : ItemData
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;
    public int minLevel;
    public int upgradeValue = 0;
    public int upgradeCount = 0;
    public int defnece=0;
    public int hp=0;
    public int mp=0;
    public int moveSpeed=0;

    public List<GameObject> charEquipment;
}
