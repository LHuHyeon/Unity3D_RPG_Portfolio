using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemData : ItemData
{
    public Define.WeaponType weaponType = Define.WeaponType.Unknown;
    public int minLevel;
    public int upgradeValue = 0;
    public int upgradeCount = 0;
    public int attack=0;

    public GameObject charEquipment;
}
