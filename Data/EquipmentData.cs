using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 장비 아이템 데이터 ]
1. 모든 장비는 해당 클래스를 상속받음.
*/

public class EquipmentData : ItemData
{
    public int minLevel;            // 최소 장착 레벨
    public int upgradeValue = 0;    // +1 강화마다 올라가는 수치
    public int upgradeCount = 0;    // 업그레이드 횟수

    public void EquipmentClone(EquipmentData equip)
    {
        equip.minLevel = this.minLevel;
        equip.upgradeValue = this.upgradeValue;
        equip.upgradeCount = this.upgradeCount;
    }
}
