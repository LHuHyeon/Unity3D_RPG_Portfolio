using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ItemData
{
    public int minLevel;
    public int upgradeValue = 0;
    public int upgradeCount = 0;

    public EquipmentData EquipmentClone()
    {
        EquipmentData equip = new EquipmentData();
        equip.minLevel = this.minLevel;
        equip.upgradeValue = this.upgradeValue;
        equip.upgradeCount = this.upgradeCount;

        return equip;
    }
}
