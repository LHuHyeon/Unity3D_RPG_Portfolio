using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[ 방어 장비 아이템 데이터 ]
1. 모든 방어 아이템은 해당 클래스를 받음.
2. 스탯 : 방어도, 체력, 마나, 속도
*/

[Serializable]
public class ArmorItemData : EquipmentData
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;

    // 기본 스탯
    public int defnece=0;
    public int hp=0;
    public int mp=0;
    public int moveSpeed=0;

    // 강화 시 추가 스탯
    public int addDefnece=0;
    public int addHp=0;
    public int addMp=0;
    public int addMoveSpeed=0;

    [NonSerialized]
    public List<GameObject> charEquipment;  // 캐릭터 파츠 활성화

    // 깊은 복사용
    public ArmorItemData ArmorClone()
    {
        ArmorItemData armor = new ArmorItemData();

        (this as EquipmentData).EquipmentClone(armor);

        armor.armorType = this.armorType;
        armor.defnece = this.defnece;
        armor.hp = this.hp;
        armor.mp = this.mp;
        armor.moveSpeed = this.moveSpeed;

        armor.charEquipment = this.charEquipment;

        return armor;
    }
}
