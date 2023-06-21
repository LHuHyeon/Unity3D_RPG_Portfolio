using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ItemData
{
    public int minLevel;
    public int upgradeValue = 0;
    public int upgradeCount = 0;

    public void EquipmentClone(EquipmentData equip)
    {
        equip.minLevel = this.minLevel;
        equip.upgradeValue = this.upgradeValue;
        equip.upgradeCount = this.upgradeCount;
    }
}
