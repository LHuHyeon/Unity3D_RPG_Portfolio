using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemData : EquipmentData
{
    public Define.WeaponType weaponType = Define.WeaponType.Unknown;
    
    public int attack=0;
    public int addAttack=0;

    public GameObject charEquipment;

    public WeaponItemData WeaponClone()
    {
        WeaponItemData weapon = new WeaponItemData();

        weapon.weaponType = this.weaponType;
        weapon.attack = this.attack;
        weapon.charEquipment = this.charEquipment;

        return weapon;
    }
}
