using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStat
{
    public Define.StatType statType; // 장비 효과
    public int vlaue; // 수치
}

public class EqItem : Item
{
    public Define.EquipmentType equipmentType = Define.EquipmentType.Unknown;
    public int minLevel;
    public int upgradeCount = 0;
    public List<EquipmentStat> eqStat = new List<EquipmentStat>();
}