using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItemData : EquipmentData
{
    public Define.ArmorType armorType = Define.ArmorType.Unknown;
    public int defnece=0;
    public int hp=0;
    public int mp=0;
    public int moveSpeed=0;

    public int addDefnece=0;
    public int addHp=0;
    public int addMp=0;
    public int addMoveSpeed=0;

    public List<GameObject> charEquipment;

    public ArmorItemData ArmorClone()
    {
        ArmorItemData armor = new ArmorItemData();
        armor.armorType = this.armorType;
        armor.defnece = this.defnece;
        armor.hp = this.hp;
        armor.mp = this.mp;
        armor.moveSpeed = this.moveSpeed;

        armor.charEquipment = this.charEquipment;

        return armor;
    }
}
